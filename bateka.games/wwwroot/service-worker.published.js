// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts("./service-worker-assets.js");
self.addEventListener("install", (event) => event.waitUntil(onInstall(event)));
self.addEventListener("activate", (event) =>
  event.waitUntil(onActivate(event)),
);
self.addEventListener("fetch", (event) => {
  // Pages that belong to Bateka.BFF, not to this app, go straight to the network. Without this the
  // worker answered /login.html with the cached app: the expired-login redirect landed back in the
  // app (404 page), the next API call got 401 again, and the page reloaded forever.
  if (isBffPage(event.request)) return;
  event.respondWith(onFetch(event));
});

const cacheNamePrefix = "offline-cache-";
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [
  /\.dll$/,
  /\.pdb$/,
  /\.wasm/,
  /\.html/,
  /\.js$/,
  /\.json$/,
  /\.css$/,
  /\.woff$/,
  /\.png$/,
  /\.jpe?g$/,
  /\.gif$/,
  /\.ico$/,
  /\.blat$/,
  /\.dat$/,
  /\.webmanifest$/,
];
const offlineAssetsExclude = [/^service-worker\.js$/];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(
  (asset) => new URL(asset.url, baseUrl).href,
);

// Bateka.BFF's own pages and endpoints (login, no access, sign out, /auth/me).
const bffPaths = ["/login.html", "/no-access.html", "/auth/"];

function isBffPage(request) {
  const path = new URL(request.url).pathname.toLowerCase();
  return bffPaths.some((p) => path.startsWith(p));
}

async function onInstall(event) {
  console.info("Service worker: Install");
  // Take over right away: a phone stuck on the old worker would otherwise keep looping until
  // every tab of the app is closed.
  self.skipWaiting();

  // Fetch and cache all matching items from the assets manifest
  const assetsRequests = self.assetsManifest.assets
    .filter((asset) =>
      offlineAssetsInclude.some((pattern) => pattern.test(asset.url)),
    )
    .filter(
      (asset) =>
        !offlineAssetsExclude.some((pattern) => pattern.test(asset.url)),
    )
    .map(
      (asset) =>
        new Request(asset.url, { integrity: asset.hash, cache: "no-cache" }),
    );
  await caches.open(cacheName).then((cache) => cache.addAll(assetsRequests));
}

async function onActivate(event) {
  console.info("Service worker: Activate");

  // Delete unused caches
  const cacheKeys = await caches.keys();
  await Promise.all(
    cacheKeys
      .filter((key) => key.startsWith(cacheNamePrefix) && key !== cacheName)
      .map((key) => caches.delete(key)),
  );

  await self.clients.claim();
}

async function onFetch(event) {
  let cachedResponse = null;
  if (event.request.method === "GET") {
    // For all navigation requests, try to serve index.html from cache,
    // unless that request is for an offline resource.
    // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
    const shouldServeIndexHtml =
      event.request.mode === "navigate" &&
      !manifestUrlList.some((url) => url === event.request.url);

    const request = shouldServeIndexHtml ? "index.html" : event.request;
    const cache = await caches.open(cacheName);
    cachedResponse = await cache.match(request);
  }

  return cachedResponse || fetch(event.request);
}
