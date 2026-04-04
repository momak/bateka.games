window.snakeDraw = {
    draw(canvas, gridSize, cellSize, snake, food, head, gameOver) {
        const ctx = canvas.getContext('2d');
        const w = gridSize * cellSize;

        // Background
        ctx.fillStyle = '#0a0a0a';
        ctx.fillRect(0, 0, w, w);

        // Grid lines
        ctx.strokeStyle = 'rgba(255,255,255,0.04)';
        ctx.lineWidth = 0.5;
        for (let i = 0; i <= gridSize; i++) {
            ctx.beginPath();
            ctx.moveTo(i * cellSize, 0);
            ctx.lineTo(i * cellSize, w);
            ctx.stroke();
            ctx.beginPath();
            ctx.moveTo(0, i * cellSize);
            ctx.lineTo(w, i * cellSize);
            ctx.stroke();
        }

        // Snake body
        snake.forEach((p, idx) => {
            const isHead = head && p.x === head.x && p.y === head.y;
            const ratio = 1 - (idx / snake.length) * 0.5;

            ctx.fillStyle = isHead
                ? '#BB86FC'
                : `rgba(98, 0, 234, ${ratio})`;

            const margin = isHead ? 1 : 2;
            this._roundRect(ctx,
                p.x * cellSize + margin,
                p.y * cellSize + margin,
                cellSize - margin * 2,
                cellSize - margin * 2,
                isHead ? 6 : 4);
        });

        // Eyes on head
        if (head) {
            ctx.fillStyle = '#ffffff';
            const hx = head.x * cellSize;
            const hy = head.y * cellSize;
            const r = 2.5;

            ctx.beginPath();
            ctx.arc(hx + cellSize * 0.3, hy + cellSize * 0.3, r, 0, Math.PI * 2);
            ctx.fill();
            ctx.beginPath();
            ctx.arc(hx + cellSize * 0.7, hy + cellSize * 0.3, r, 0, Math.PI * 2);
            ctx.fill();
        }

        // Food
        food.forEach(f => {
            const fx = f.x * cellSize + cellSize / 2;
            const fy = f.y * cellSize + cellSize / 2;
            const r = cellSize / 2 - 3;

            // Glow
            const grd = ctx.createRadialGradient(fx, fy, 1, fx, fy, r);
            grd.addColorStop(0, '#ff6d6d');
            grd.addColorStop(1, '#c62828');
            ctx.fillStyle = grd;

            ctx.beginPath();
            ctx.arc(fx, fy, r, 0, Math.PI * 2);
            ctx.fill();

            // Shine
            ctx.fillStyle = 'rgba(255,255,255,0.35)';
            ctx.beginPath();
            ctx.arc(fx - r * 0.25, fy - r * 0.25, r * 0.35, 0, Math.PI * 2);
            ctx.fill();
        });
    },

    drawBoard(canvas, gridSize, cellSize) {
        const ctx = canvas.getContext('2d');
        const w = gridSize * cellSize;

        ctx.fillStyle = '#0a0a0a';
        ctx.fillRect(0, 0, w, w);

        ctx.strokeStyle = 'rgba(255,255,255,0.04)';
        ctx.lineWidth = 0.5;
        for (let i = 0; i <= gridSize; i++) {
            ctx.beginPath();
            ctx.moveTo(i * cellSize, 0);
            ctx.lineTo(i * cellSize, w);
            ctx.stroke();
            ctx.beginPath();
            ctx.moveTo(0, i * cellSize);
            ctx.lineTo(w, i * cellSize);
            ctx.stroke();
        }
    },

    _roundRect(ctx, x, y, w, h, r) {
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
};