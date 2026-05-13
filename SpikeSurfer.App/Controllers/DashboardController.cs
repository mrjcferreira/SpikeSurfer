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
<html>
<head>
<title>SpikeSurfer Dashboard</title>
<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
<style>
body{background:#0b0b0b;color:white;font-family:Arial;padding:20px;}
.card{background:#151515;border-radius:16px;padding:20px;margin-bottom:20px;}
.value{font-size:42px;font-weight:bold;}
.green{color:#00ff88;}
.red{color:#ff5555;}
canvas{background:#111;border-radius:12px;padding:10px;}
@keyframes flashGreen{0%{color:#00ff88;}50%{color:white;}100%{color:#00ff88;}}
@keyframes flashRed{0%{color:#ff4444;}50%{color:white;}100%{color:#ff4444;}}
.flashGreen{animation:flashGreen 0.5s infinite;}
.flashRed{animation:flashRed 0.5s infinite;}
</style>
</head>
<body>
<h1>SpikeSurfer Dashboard</h1>

<div class='card'>
    <div>XAUUSD</div>
    <div class='value' id='price'>--</div>
    <div id='change'>--</div>
</div>

<div class='card'>
    <canvas id='priceChart'></canvas>
</div>

<script>
let labels = [];
let values = [];

const ctx = document.getElementById('priceChart');

const chart = new Chart(ctx,{
    type:'line',
    data:{
        labels:labels,
        datasets:[{
            label:'Gold',
            data:values
        }]
    }
});

async function update(){
    const r = await fetch('/api/market/xauusd');
    const d = await r.json();

    if(d.error){ return; }

    const priceEl = document.getElementById('price');
    priceEl.innerText = d.mid.toFixed(2);

    const pct = d.change1hPercent.toFixed(3);

    document.getElementById('change').innerHTML =
        d.change1hPercent >= 0
        ? '<span class=""green"">▲ '+pct+'%</span>'
        : '<span class=""red"">▼ '+pct+'%</span>';

    priceEl.classList.remove('flashGreen');
    priceEl.classList.remove('flashRed');

    if(d.change1hPercent >= 0.1){
        priceEl.classList.add('flashGreen');
    }

    if(d.change1hPercent <= -0.1){
        priceEl.classList.add('flashRed');
    }

    labels.push(new Date().toLocaleTimeString());
    values.push(d.mid);

    if(labels.length > 30){
        labels.shift();
        values.shift();
    }

    chart.update();
}

setInterval(update,2000);
update();
</script>
</body>
</html>
";

        return Content(html, "text/html");
    }
}