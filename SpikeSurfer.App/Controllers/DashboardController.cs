using Microsoft.AspNetCore.Mvc;

namespace SpikeSurfer.App.Controllers;

[ApiController]
public class DashboardController : ControllerBase
{
    [HttpGet("/dashboard")]
    public ContentResult Dashboard()
    {
        var html = @"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>SpikeSurfer Dashboard</title>
<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
<link rel='preconnect' href='https://fonts.googleapis.com'>
<link href='https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&family=Outfit:wght@300;500;700&display=swap' rel='stylesheet'>
<style>
:root {
    --bg-primary: #08090d;
    --bg-card: #0f1117;
    --bg-card-hover: #14161e;
    --border: #1a1d2a;
    --text-primary: #e8eaf0;
    --text-secondary: #6b7084;
    --text-muted: #3d4156;
    --accent-green: #22d68a;
    --accent-green-dim: rgba(34, 214, 138, 0.12);
    --accent-red: #f04668;
    --accent-red-dim: rgba(240, 70, 104, 0.12);
    --accent-blue: #3b82f6;
    --accent-gold: #f5b731;
    --accent-gold-dim: rgba(245, 183, 49, 0.08);
    --font-mono: 'JetBrains Mono', monospace;
    --font-display: 'Outfit', sans-serif;
}

* { margin: 0; padding: 0; box-sizing: border-box; }

body {
    background: var(--bg-primary);
    color: var(--text-primary);
    font-family: var(--font-display);
    min-height: 100vh;
    overflow-x: hidden;
}

/* === GRAIN OVERLAY === */
body::before {
    content: '';
    position: fixed; inset: 0;
    background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 200 200""><filter id=""n""><feTurbulence type=""fractalNoise"" baseFrequency=""0.9"" numOctaves=""4"" stitchTiles=""stitch""/></filter><rect width=""200"" height=""200"" filter=""url(%23n)"" opacity=""0.03""/></svg>');
    pointer-events: none;
    z-index: 9999;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 24px 20px;
}

/* === HEADER === */
.header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 28px;
    padding-bottom: 20px;
    border-bottom: 1px solid var(--border);
}

.header-left {
    display: flex;
    align-items: center;
    gap: 14px;
}

.logo {
    width: 36px; height: 36px;
    background: linear-gradient(135deg, var(--accent-gold), #e8940a);
    border-radius: 10px;
    display: flex; align-items: center; justify-content: center;
    font-weight: 700; font-size: 16px; color: #000;
    font-family: var(--font-mono);
    box-shadow: 0 0 20px rgba(245, 183, 49, 0.15);
}

.header h1 {
    font-size: 20px;
    font-weight: 700;
    letter-spacing: -0.3px;
    color: var(--text-primary);
}

.header h1 span {
    color: var(--accent-gold);
}

.connection-status {
    display: flex; align-items: center; gap: 8px;
    font-size: 12px; font-family: var(--font-mono);
    color: var(--text-secondary);
}

.status-dot {
    width: 8px; height: 8px;
    border-radius: 50%;
    background: var(--text-muted);
    transition: background 0.3s;
}

.status-dot.live {
    background: var(--accent-green);
    box-shadow: 0 0 8px rgba(34, 214, 138, 0.4);
    animation: pulse 2s infinite;
}

@keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.5; }
}

/* === GRID === */
.grid {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 16px;
    margin-bottom: 16px;
}

/* === CARDS === */
.card {
    background: var(--bg-card);
    border: 1px solid var(--border);
    border-radius: 14px;
    padding: 20px;
    transition: background 0.2s;
}

.card:hover {
    background: var(--bg-card-hover);
}

.card-label {
    font-size: 11px;
    font-weight: 500;
    text-transform: uppercase;
    letter-spacing: 1.2px;
    color: var(--text-secondary);
    margin-bottom: 10px;
}

/* === PRICE CARD === */
.price-card {
    grid-column: span 2;
    display: flex;
    align-items: flex-end;
    justify-content: space-between;
}

.price-main {
    display: flex;
    flex-direction: column;
}

.symbol {
    font-size: 13px;
    font-family: var(--font-mono);
    font-weight: 600;
    color: var(--accent-gold);
    letter-spacing: 2px;
    margin-bottom: 6px;
}

.price-value {
    font-family: var(--font-mono);
    font-size: 48px;
    font-weight: 700;
    letter-spacing: -2px;
    line-height: 1;
    transition: color 0.3s;
}

.price-value.up { color: var(--accent-green); }
.price-value.down { color: var(--accent-red); }

.price-change {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    font-family: var(--font-mono);
    font-size: 14px;
    font-weight: 600;
    margin-top: 10px;
    padding: 5px 12px;
    border-radius: 8px;
    transition: all 0.3s;
}

.price-change.up {
    color: var(--accent-green);
    background: var(--accent-green-dim);
}

.price-change.down {
    color: var(--accent-red);
    background: var(--accent-red-dim);
}

.price-meta {
    text-align: right;
    font-family: var(--font-mono);
    font-size: 12px;
    color: var(--text-secondary);
    line-height: 1.8;
}

/* === STAT CARD === */
.stat-value {
    font-family: var(--font-mono);
    font-size: 28px;
    font-weight: 700;
    letter-spacing: -1px;
}

.stat-sub {
    font-size: 12px;
    color: var(--text-secondary);
    margin-top: 4px;
    font-family: var(--font-mono);
}

/* === CHART === */
.chart-card {
    grid-column: span 3;
    padding: 20px;
    position: relative;
}

.chart-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
}

.chart-tabs {
    display: flex;
    gap: 4px;
}

.chart-tab {
    font-family: var(--font-mono);
    font-size: 11px;
    padding: 6px 12px;
    border-radius: 6px;
    border: 1px solid var(--border);
    background: transparent;
    color: var(--text-secondary);
    cursor: pointer;
    transition: all 0.2s;
}

.chart-tab:hover { border-color: var(--text-muted); color: var(--text-primary); }
.chart-tab.active { background: var(--accent-gold-dim); border-color: var(--accent-gold); color: var(--accent-gold); }

.chart-wrapper {
    position: relative;
    height: 320px;
}

canvas { border-radius: 8px; }

/* === SESSION BAR === */
.session-bar {
    grid-column: span 3;
    display: flex;
    gap: 12px;
}

.session-item {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 14px 18px;
    border-radius: 12px;
    background: var(--bg-card);
    border: 1px solid var(--border);
    font-family: var(--font-mono);
    font-size: 12px;
}

.session-dot {
    width: 10px; height: 10px;
    border-radius: 50%;
    background: var(--text-muted);
    flex-shrink: 0;
}

.session-dot.active {
    background: var(--accent-green);
    box-shadow: 0 0 10px rgba(34, 214, 138, 0.3);
}

.session-label { color: var(--text-secondary); }
.session-time { color: var(--text-primary); margin-left: auto; }

/* === DATA GRID === */
.data-grid {
    grid-column: span 3;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 12px;
}

.data-cell {
    background: var(--bg-card);
    border: 1px solid var(--border);
    border-radius: 10px;
    padding: 14px 16px;
}

.data-cell-label {
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 1px;
    color: var(--text-muted);
    margin-bottom: 6px;
    font-family: var(--font-mono);
}

.data-cell-value {
    font-family: var(--font-mono);
    font-size: 16px;
    font-weight: 600;
    color: var(--text-primary);
}

/* === NO DATA STATE === */
.no-data {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 60px;
    color: var(--text-muted);
    font-family: var(--font-mono);
    font-size: 14px;
    grid-column: span 3;
}

.no-data .spinner {
    width: 24px; height: 24px;
    border: 2px solid var(--border);
    border-top-color: var(--accent-gold);
    border-radius: 50%;
    animation: spin 0.8s linear infinite;
    margin-bottom: 14px;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* === RESPONSIVE === */
@media (max-width: 768px) {
    .grid { grid-template-columns: 1fr; }
    .price-card,
    .chart-card,
    .session-bar,
    .data-grid { grid-column: span 1; }
    .data-grid { grid-template-columns: repeat(2, 1fr); }
    .price-value { font-size: 36px; }
    .price-card { flex-direction: column; align-items: flex-start; gap: 12px; }
    .price-meta { text-align: left; }
}
</style>
</head>
<body>
<div class='container'>

<div class='header'>
    <div class='header-left'>
        <div class='logo'>SS</div>
        <h1>Spike<span>Surfer</span></h1>
    </div>
    <div class='connection-status'>
        <div class='status-dot' id='statusDot'></div>
        <span id='statusText'>Connecting...</span>
    </div>
</div>

<div class='grid' id='mainGrid'>
    <div class='no-data' id='noData'>
        <div class='spinner'></div>
        Waiting for market data...
    </div>
</div>

</div>

<script>
const MAX_POINTS = { '1m': 60, '5m': 60, '15m': 40 };
const POLL_MS = 2000;

let labels = [];
let values = [];
let highValues = [];
let lowValues = [];
let currentRange = '1m';
let lastMid = null;
let lastDirection = null;
let dataReceived = false;
let chart = null;

function createDashboard() {
    document.getElementById('mainGrid').innerHTML = `
        <div class='card price-card'>
            <div class='price-main'>
                <div class='symbol'>XAUUSD</div>
                <div class='price-value' id='price'>--</div>
                <div class='price-change' id='change'>--</div>
            </div>
            <div class='price-meta'>
                <div>BID <span id='bid'>--</span></div>
                <div>ASK <span id='ask'>--</span></div>
                <div>SPREAD <span id='spread'>--</span></div>
            </div>
        </div>

        <div class='card'>
            <div class='card-label'>Session</div>
            <div class='stat-value' id='session'>--</div>
            <div class='stat-sub' id='serverTime'>--</div>
        </div>

        <div class='chart-card card'>
            <div class='chart-header'>
                <div class='card-label' style='margin-bottom:0'>Price</div>
                <div class='chart-tabs'>
                    <button class='chart-tab active' onclick='setRange(""1m"",this)'>1M</button>
                    <button class='chart-tab' onclick='setRange(""5m"",this)'>5M</button>
                    <button class='chart-tab' onclick='setRange(""15m"",this)'>15M</button>
                </div>
            </div>
            <div class='chart-wrapper'>
                <canvas id='priceChart'></canvas>
            </div>
        </div>

        <div class='data-grid'>
            <div class='data-cell'>
                <div class='data-cell-label'>1h Change</div>
                <div class='data-cell-value' id='change1h'>--</div>
            </div>
            <div class='data-cell'>
                <div class='data-cell-label'>1h %</div>
                <div class='data-cell-value' id='change1hPct'>--</div>
            </div>
            <div class='data-cell'>
                <div class='data-cell-label'>High (session)</div>
                <div class='data-cell-value' id='sessionHigh'>--</div>
            </div>
            <div class='data-cell'>
                <div class='data-cell-label'>Low (session)</div>
                <div class='data-cell-value' id='sessionLow'>--</div>
            </div>
        </div>

        <div class='session-bar'>
            <div class='session-item'>
                <div class='session-dot' id='dotAsia'></div>
                <span class='session-label'>Asia</span>
                <span class='session-time'>00:00–07:00</span>
            </div>
            <div class='session-item'>
                <div class='session-dot' id='dotLondon'></div>
                <span class='session-label'>London</span>
                <span class='session-time'>07:00–16:00</span>
            </div>
            <div class='session-item'>
                <div class='session-dot' id='dotNewYork'></div>
                <span class='session-label'>New York</span>
                <span class='session-time'>12:00–21:00</span>
            </div>
        </div>
    `;

    initChart();
}

function initChart() {
    const ctx = document.getElementById('priceChart');

    const gradient = ctx.getContext('2d');
    const grad = gradient.createLinearGradient(0, 0, 0, 320);
    grad.addColorStop(0, 'rgba(245, 183, 49, 0.15)');
    grad.addColorStop(1, 'rgba(245, 183, 49, 0)');

    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'XAUUSD',
                data: values,
                borderColor: '#f5b731',
                borderWidth: 2,
                backgroundColor: grad,
                fill: true,
                tension: 0.3,
                pointRadius: 0,
                pointHitRadius: 8,
                pointHoverRadius: 4,
                pointHoverBackgroundColor: '#f5b731',
                pointHoverBorderColor: '#fff',
                pointHoverBorderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: 'index'
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: '#1a1d2a',
                    titleColor: '#6b7084',
                    bodyColor: '#e8eaf0',
                    titleFont: { family: 'JetBrains Mono', size: 11 },
                    bodyFont: { family: 'JetBrains Mono', size: 13, weight: '600' },
                    padding: 12,
                    cornerRadius: 8,
                    borderColor: '#2a2d3a',
                    borderWidth: 1,
                    displayColors: false,
                    callbacks: {
                        label: function(ctx) { return ctx.parsed.y.toFixed(2); }
                    }
                }
            },
            scales: {
                x: {
                    grid: { color: 'rgba(255,255,255,0.03)', drawBorder: false },
                    ticks: {
                        color: '#3d4156',
                        font: { family: 'JetBrains Mono', size: 10 },
                        maxTicksLimit: 8,
                        maxRotation: 0
                    }
                },
                y: {
                    position: 'right',
                    grid: { color: 'rgba(255,255,255,0.03)', drawBorder: false },
                    ticks: {
                        color: '#3d4156',
                        font: { family: 'JetBrains Mono', size: 10 },
                        callback: function(v) { return v.toFixed(1); }
                    }
                }
            }
        }
    });
}

function setRange(range, el) {
    currentRange = range;
    labels.length = 0;
    values.length = 0;
    if (chart) chart.update('none');
    document.querySelectorAll('.chart-tab').forEach(t => t.classList.remove('active'));
    el.classList.add('active');
}

function getActiveSession(h) {
    const sessions = [];
    if (h >= 0 && h < 7) sessions.push('asia');
    if (h >= 7 && h < 16) sessions.push('london');
    if (h >= 12 && h < 21) sessions.push('newyork');
    return sessions;
}

let sessionHigh = -Infinity;
let sessionLow = Infinity;

async function update() {
    try {
        const r = await fetch('/api/market/xauusd');

        if (!r.ok) {
            document.getElementById('statusDot').classList.remove('live');
            document.getElementById('statusText').textContent = 'No data';
            return;
        }

        const d = await r.json();

        if (!dataReceived) {
            dataReceived = true;
            createDashboard();
        }

        // Connection status
        document.getElementById('statusDot').classList.add('live');
        document.getElementById('statusText').textContent = 'LIVE';

        // Price direction
        const direction = lastMid !== null ? (d.mid > lastMid ? 'up' : d.mid < lastMid ? 'down' : lastDirection) : null;
        lastDirection = direction;
        lastMid = d.mid;

        // Price display
        const priceEl = document.getElementById('price');
        priceEl.textContent = d.mid.toFixed(2);
        priceEl.className = 'price-value' + (direction ? ' ' + direction : '');

        // Change badge
        const changeEl = document.getElementById('change');
        const pct = d.change1hPercent.toFixed(3);
        const isUp = d.change1hPercent >= 0;
        changeEl.textContent = (isUp ? '\u25B2 +' : '\u25BC ') + pct + '%';
        changeEl.className = 'price-change ' + (isUp ? 'up' : 'down');

        // Meta
        document.getElementById('bid').textContent = d.bid.toFixed(2);
        document.getElementById('ask').textContent = d.ask.toFixed(2);
        document.getElementById('spread').textContent = d.spread.toFixed(1) + ' pts';

        // Session
        const serverTime = new Date(d.serverTimeUtc);
        const hour = serverTime.getUTCHours();
        const activeSessions = getActiveSession(hour);

        let sessionLabel = activeSessions.length === 0 ? 'Closed' :
            activeSessions.map(s => s === 'asia' ? 'Asia' : s === 'london' ? 'London' : 'New York').join(' / ');
        document.getElementById('session').textContent = sessionLabel;
        document.getElementById('serverTime').textContent = serverTime.toISOString().slice(11, 19) + ' UTC';

        ['asia', 'london', 'newyork'].forEach(s => {
            const dot = document.getElementById('dot' + s.charAt(0).toUpperCase() + s.slice(1));
            if (dot) {
                if (activeSessions.includes(s)) dot.classList.add('active');
                else dot.classList.remove('active');
            }
        });

        // Data grid
        const ch1h = d.change1h;
        document.getElementById('change1h').textContent = (ch1h >= 0 ? '+' : '') + ch1h.toFixed(2);
        document.getElementById('change1h').style.color = ch1h >= 0 ? 'var(--accent-green)' : 'var(--accent-red)';
        document.getElementById('change1hPct').textContent = (d.change1hPercent >= 0 ? '+' : '') + d.change1hPercent.toFixed(3) + '%';
        document.getElementById('change1hPct').style.color = d.change1hPercent >= 0 ? 'var(--accent-green)' : 'var(--accent-red)';

        if (d.mid > sessionHigh) sessionHigh = d.mid;
        if (d.mid < sessionLow) sessionLow = d.mid;
        document.getElementById('sessionHigh').textContent = sessionHigh.toFixed(2);
        document.getElementById('sessionLow').textContent = sessionLow.toFixed(2);

        // Chart
        const now = new Date();
        const timeLabel = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });

        labels.push(timeLabel);
        values.push(d.mid);

        const max = MAX_POINTS[currentRange] || 60;
        while (labels.length > max) {
            labels.shift();
            values.shift();
        }

        if (chart) chart.update('none');

    } catch (e) {
        const dot = document.getElementById('statusDot');
        const txt = document.getElementById('statusText');
        if (dot) dot.classList.remove('live');
        if (txt) txt.textContent = 'Disconnected';
    }
}

setInterval(update, POLL_MS);
update();
</script>
</body>
</html>
";

        return Content(html, "text/html");
    }
}
