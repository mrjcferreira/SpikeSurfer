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
<style>
body { margin:0; background:#08090d; color:#e8eaf0; font-family:Arial, sans-serif; }
.container { max-width:1200px; margin:0 auto; padding:24px; }
.header { display:flex; justify-content:space-between; align-items:center; border-bottom:1px solid #1a1d2a; padding-bottom:18px; margin-bottom:20px; }
.logo { color:#f5b731; font-weight:700; font-size:22px; }
.status { color:#6b7084; font-size:13px; }
.grid { display:grid; grid-template-columns:repeat(3,1fr); gap:16px; }
.card { background:#0f1117; border:1px solid #1a1d2a; border-radius:14px; padding:20px; }
.card-wide { grid-column:span 3; }
.price-card { grid-column:span 2; }
.label { color:#6b7084; font-size:11px; text-transform:uppercase; letter-spacing:1px; margin-bottom:10px; }
.price { font-size:46px; font-weight:700; font-family:monospace; }
.green { color:#22d68a; }
.red { color:#f04668; }
.gold { color:#f5b731; }
.mono { font-family:monospace; }
.small { color:#6b7084; font-size:13px; line-height:1.7; }
.chart-wrap { height:320px; }
.data-grid { grid-column:span 3; display:grid; grid-template-columns:repeat(4,1fr); gap:12px; }
.data-cell { background:#0f1117; border:1px solid #1a1d2a; border-radius:10px; padding:14px; }
@media(max-width:768px){ .grid,.data-grid{grid-template-columns:1fr;} .card-wide,.price-card,.data-grid{grid-column:span 1;} }
</style>
</head>
<body>
<div class='container'>
    <div class='header'>
        <div class='logo'>SpikeSurfer</div>
        <div class='status' id='statusText'>Starting...</div>
    </div>

    <div class='grid' id='mainGrid'>
        <div class='card price-card'>
            <div class='label'>XAUUSD</div>
            <div class='price' id='price'>--</div>
            <div class='small'>Bid: <span id='bid'>--</span> | Ask: <span id='ask'>--</span> | Spread: <span id='spread'>--</span></div>
        </div>

        <div class='card'>
            <div class='label'>Session</div>
            <div class='price' style='font-size:28px' id='session'>--</div>
            <div class='small' id='serverTime'>--</div>
        </div>

        <div class='card card-wide'>
            <div class='label'>Market Structure</div>
            <div id='narrativeBox' class='small'>Loading narrative...</div>
        </div>

        <div class='card card-wide'>
            <div class='label'>Price Chart</div>
            <div class='chart-wrap'><canvas id='priceChart'></canvas></div>
        </div>

        <div class='data-grid'>
            <div class='data-cell'><div class='label'>1h Change</div><div id='change1h'>--</div></div>
            <div class='data-cell'><div class='label'>1h %</div><div id='change1hPct'>--</div></div>
            <div class='data-cell'><div class='label'>Session High</div><div id='sessionHigh'>--</div></div>
            <div class='data-cell'><div class='label'>Session Low</div><div id='sessionLow'>--</div></div>
        </div>
    </div>
</div>

<script>
const POLL_MS = 2000;
const MAX_POINTS = 60;

let labels = [];
let values = [];
let chart = null;
let lastMid = null;
let sessionHigh = -Infinity;
let sessionLow = Infinity;

function initChart() {
    const ctx = document.getElementById('priceChart');
    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'XAUUSD',
                data: values,
                borderColor: '#f5b731',
                borderWidth: 2,
                tension: 0.3,
                pointRadius: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: {
                x: { ticks: { color: '#6b7084' }, grid: { color: 'rgba(255,255,255,0.04)' } },
                y: { position: 'right', ticks: { color: '#6b7084' }, grid: { color: 'rgba(255,255,255,0.04)' } }
            }
        }
    });
}

function getActiveSession(h) {
    const sessions = [];
    if (h >= 0 && h < 7) sessions.push('Asia');
    if (h >= 7 && h < 16) sessions.push('London');
    if (h >= 12 && h < 21) sessions.push('New York');
    return sessions.length ? sessions.join(' / ') : 'Closed';
}

async function updateNarrative() {
    try {
        const r = await fetch('/api/dashboard/narrative');
        if (!r.ok) return;

        const n = await r.json();
        const el = document.getElementById('narrativeBox');

        el.innerHTML = `
            <div class='gold' style='font-size:22px;font-weight:700;margin-bottom:10px;'>${n.structure}</div>
            <div>Phase: ${n.phase}</div>
            <div>Bias: ${n.bias}</div>
            <div>Energy: ${n.energy}</div>
            <div>Confidence: ${n.confidence}%</div>
            <div>ETA: ${n.etaCandles} candles</div>
            <p style='color:#e8eaf0;margin-top:12px;'>${n.narrative}</p>
        `;
    } catch {
        document.getElementById('narrativeBox').textContent = 'Narrative unavailable';
    }
}

async function updateMarket() {
    try {
        const r = await fetch('/api/market/xauusd');

        if (!r.ok) {
            document.getElementById('statusText').textContent = 'No market data';
            return;
        }

        const d = await r.json();

        document.getElementById('statusText').textContent = 'LIVE';

        const priceEl = document.getElementById('price');
        priceEl.textContent = d.mid.toFixed(2);

        if (lastMid !== null) {
            priceEl.className = 'price ' + (d.mid >= lastMid ? 'green' : 'red');
        }

        lastMid = d.mid;

        document.getElementById('bid').textContent = d.bid.toFixed(2);
        document.getElementById('ask').textContent = d.ask.toFixed(2);
        document.getElementById('spread').textContent = d.spread.toFixed(1);

        const serverTime = new Date(d.serverTimeUtc);
        document.getElementById('session').textContent = getActiveSession(serverTime.getUTCHours());
        document.getElementById('serverTime').textContent = serverTime.toISOString().slice(11, 19) + ' UTC';

        document.getElementById('change1h').textContent = d.change1h.toFixed(2);
        document.getElementById('change1hPct').textContent = d.change1hPercent.toFixed(3) + '%';

        if (d.mid > sessionHigh) sessionHigh = d.mid;
        if (d.mid < sessionLow) sessionLow = d.mid;

        document.getElementById('sessionHigh').textContent = sessionHigh.toFixed(2);
        document.getElementById('sessionLow').textContent = sessionLow.toFixed(2);

        labels.push(new Date().toLocaleTimeString());
        values.push(d.mid);

        while (labels.length > MAX_POINTS) {
            labels.shift();
            values.shift();
        }

        if (chart) chart.update('none');
    } catch {
        document.getElementById('statusText').textContent = 'Disconnected';
    }
}

async function update() {
    await updateNarrative();
    await updateMarket();
}

initChart();
update();
setInterval(update, POLL_MS);
</script>
</body>
</html>
";

        return Content(html, "text/html");
    }
}