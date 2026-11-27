// Snake's canvas drawing — an ES module, loaded by Interop/SnakeCanvas.cs (no globals, no eval).
// frame: { gridSize, cellSize, body: [{x, y}], food: [{x, y}], head: {x, y} | null, isOver }

const colors = {
    board: '#0a0a0a',
    gridLine: 'rgba(255,255,255,0.04)',
    head: '#BB86FC',
    body: (alpha) => `rgba(98, 0, 234, ${alpha})`,
    eye: '#ffffff',
    foodInner: '#ff6d6d',
    foodOuter: '#c62828',
    foodShine: 'rgba(255,255,255,0.35)'
};

export function isTouchDevice() {
    return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
}

// ── Swipe to steer ──────────────────────────────────────────
// A turn fires as soon as the finger has moved far enough, not when it lifts — so one stroke can
// make several turns (right, then down…) and the snake reacts while you're still touching.

const swipeMinPx = 24;
const swipeHandlers = new WeakMap();

export function attachSwipe(element, receiver) {
    detachSwipe(element);

    let from = null;

    const onStart = (e) => {
        const t = e.changedTouches[0];
        from = { x: t.clientX, y: t.clientY };
    };

    const onMove = (e) => {
        if (!from) return;
        // the board is the controller: no page scroll or pull-to-refresh while steering
        e.preventDefault();

        const t = e.changedTouches[0];
        const dx = t.clientX - from.x;
        const dy = t.clientY - from.y;
        if (Math.max(Math.abs(dx), Math.abs(dy)) < swipeMinPx) return;

        const direction = Math.abs(dx) > Math.abs(dy)
            ? (dx > 0 ? 'right' : 'left')
            : (dy > 0 ? 'down' : 'up');

        from = { x: t.clientX, y: t.clientY };
        receiver.invokeMethodAsync('OnSwipe', direction);
    };

    const onEnd = () => { from = null; };

    element.addEventListener('touchstart', onStart, { passive: true });
    element.addEventListener('touchmove', onMove, { passive: false });
    element.addEventListener('touchend', onEnd, { passive: true });
    element.addEventListener('touchcancel', onEnd, { passive: true });
    swipeHandlers.set(element, { onStart, onMove, onEnd });
}

export function detachSwipe(element) {
    const handlers = element && swipeHandlers.get(element);
    if (!handlers) return;

    element.removeEventListener('touchstart', handlers.onStart);
    element.removeEventListener('touchmove', handlers.onMove);
    element.removeEventListener('touchend', handlers.onEnd);
    element.removeEventListener('touchcancel', handlers.onEnd);
    swipeHandlers.delete(element);
}

export function draw(canvas, frame) {
    const ctx = canvas.getContext('2d');
    drawBoard(ctx, frame.gridSize, frame.cellSize);
    drawSnake(ctx, frame);
    drawFood(ctx, frame);
}

function drawBoard(ctx, gridSize, cellSize) {
    const size = gridSize * cellSize;

    ctx.fillStyle = colors.board;
    ctx.fillRect(0, 0, size, size);

    ctx.strokeStyle = colors.gridLine;
    ctx.lineWidth = 0.5;
    for (let i = 0; i <= gridSize; i++) {
        line(ctx, i * cellSize, 0, i * cellSize, size);
        line(ctx, 0, i * cellSize, size, i * cellSize);
    }
}

function drawSnake(ctx, { body, head, cellSize }) {
    body.forEach((p, index) => {
        const isHead = head && p.x === head.x && p.y === head.y;
        const fade = 1 - (index / body.length) * 0.5;
        const margin = isHead ? 1 : 2;

        ctx.fillStyle = isHead ? colors.head : colors.body(fade);
        roundRect(ctx,
            p.x * cellSize + margin,
            p.y * cellSize + margin,
            cellSize - margin * 2,
            cellSize - margin * 2,
            isHead ? 6 : 4);
    });

    if (!head) return;

    ctx.fillStyle = colors.eye;
    const hx = head.x * cellSize;
    const hy = head.y * cellSize;
    dot(ctx, hx + cellSize * 0.3, hy + cellSize * 0.3, 2.5);
    dot(ctx, hx + cellSize * 0.7, hy + cellSize * 0.3, 2.5);
}

function drawFood(ctx, { food, cellSize }) {
    food.forEach(f => {
        const fx = f.x * cellSize + cellSize / 2;
        const fy = f.y * cellSize + cellSize / 2;
        const r = cellSize / 2 - 3;

        const glow = ctx.createRadialGradient(fx, fy, 1, fx, fy, r);
        glow.addColorStop(0, colors.foodInner);
        glow.addColorStop(1, colors.foodOuter);
        ctx.fillStyle = glow;
        dot(ctx, fx, fy, r);

        ctx.fillStyle = colors.foodShine;
        dot(ctx, fx - r * 0.25, fy - r * 0.25, r * 0.35);
    });
}

function line(ctx, x1, y1, x2, y2) {
    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
}

function dot(ctx, x, y, r) {
    ctx.beginPath();
    ctx.arc(x, y, r, 0, Math.PI * 2);
    ctx.fill();
}

function roundRect(ctx, x, y, w, h, r) {
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + w - r, y);
    ctx.quadraticCurveTo(x + w, y, x + w, y + r);
    ctx.lineTo(x + w, y + h - r);
    ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
    ctx.lineTo(x + r, y + h);
    ctx.quadraticCurveTo(x, y + h, x, y + h - r);
    ctx.lineTo(x, y + r);
    ctx.quadraticCurveTo(x, y, x + r, y);
    ctx.closePath();
    ctx.fill();
}
