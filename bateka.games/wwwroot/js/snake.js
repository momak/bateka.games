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
