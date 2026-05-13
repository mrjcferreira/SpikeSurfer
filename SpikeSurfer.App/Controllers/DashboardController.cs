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
<link href='https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&family=Outfit:wght@300;500;700&display=swap' rel='stylesheet'>
<style>
:root{--bg:#08090d;--card:#0f1117;--card2:#14161e;--border:#1a1d2a;--t1:#e8eaf0;--t2:#6b7084;--t3:#3d4156;--green:#22d68a;--green-d:rgba(34,214,138,.12);--red:#f04668;--red-d:rgba(240,70,104,.12);--gold:#f5b731;--gold-d:rgba(245,183,49,.08);--blue:#3b82f6;--mono:'JetBrains Mono',monospace;--sans:'Outfit',sans-serif}
*{margin:0;padding:0;box-sizing:border-box}
body{background:var(--bg);color:var(--t1);font-family:var(--sans);min-height:100vh;overflow-x:hidden}
.c{max-width:1200px;margin:0 auto;padding:24px 20px}
.hdr{display:flex;align-items:center;justify-content:space-between;margin-bottom:28px;padding-bottom:20px;border-bottom:1px solid var(--border)}
.hdr-l{display:flex;align-items:center;gap:14px}
.logo{width:36px;height:36px;background:linear-gradient(135deg,var(--gold),#e8940a);border-radius:10px;display:flex;align-items:center;justify-content:center;font:700 16px var(--mono);color:#000;box-shadow:0 0 20px rgba(245,183,49,.15)}
.hdr h1{font-size:20px;font-weight:700;letter-spacing:-.3px}
.hdr h1 span{color:var(--gold)}
.conn{display:flex;align-items:center;gap:8px;font:12px var(--mono);color:var(--t2)}
.dot{width:8px;height:8px;border-radius:50%;background:var(--t3);transition:.3s}
.dot.on{background:var(--green);box-shadow:0 0 8px rgba(34,214,138,.4);animation:pulse 2s infinite}
@keyframes pulse{0%,100%{opacity:1}50%{opacity:.5}}

.g{display:grid;grid-template-columns:1fr 1fr 1fr;gap:16px;margin-bottom:16px}
.cd{background:var(--card);border:1px solid var(--border);border-radius:14px;padding:20px;transition:.2s}
.cd:hover{background:var(--card2)}
.cl{font:500 11px var(--sans);text-transform:uppercase;letter-spacing:1.2px;color:var(--t2);margin-bottom:10px}

.pc{grid-column:span 2;display:flex;align-items:flex-end;justify-content:space-between}
.sym{font:600 13px var(--mono);color:var(--gold);letter-spacing:2px;margin-bottom:6px}
.pv{font:700 48px var(--mono);letter-spacing:-2px;line-height:1;transition:.3s}
.pv.up{color:var(--green)}.pv.down{color:var(--red)}
.pch{display:inline-flex;align-items:center;gap:6px;font:600 14px var(--mono);margin-top:10px;padding:5px 12px;border-radius:8px}
.pch.up{color:var(--green);background:var(--green-d)}.pch.down{color:var(--red);background:var(--red-d)}
.pm{text-align:right;font:12px var(--mono);color:var(--t2);line-height:1.8}
.sv{font:700 28px var(--mono);letter-spacing:-1px}
.ss{font:12px var(--mono);color:var(--t2);margin-top:4px}

/* Chart */
.cc{grid-column:span 3;padding:20px}
.ch{display:flex;align-items:center;justify-content:space-between;margin-bottom:16px}
.tabs{display:flex;gap:4px}
.tab{font:11px var(--mono);padding:6px 12px;border-radius:6px;border:1px solid var(--border);background:0;color:var(--t2);cursor:pointer;transition:.2s}
.tab:hover{border-color:var(--t3);color:var(--t1)}
.tab.on{background:var(--gold-d);border-color:var(--gold);color:var(--gold)}
.cw{position:relative;height:360px;background:#0a0b0f;border-radius:8px;overflow:hidden}
.cw canvas{width:100%!important;height:100%!important}
.lw{position:relative;height:180px}
.ctt{position:absolute;display:none;background:#1a1d2aee;border:1px solid #2a2d3a;border-radius:8px;padding:10px 14px;font:11px var(--mono);color:var(--t1);pointer-events:none;z-index:100;line-height:1.7;white-space:nowrap}

/* Sentiment */
.sent-card{grid-column:span 3;display:grid;grid-template-columns:280px 1fr;gap:20px;padding:20px}
.sent-gauge{display:flex;flex-direction:column;align-items:center;justify-content:center;gap:12px}
.sent-badge{font:700 18px var(--mono);padding:10px 24px;border-radius:12px;letter-spacing:1px}
.sent-badge.BULLISH{color:var(--green);background:var(--green-d);border:1px solid rgba(34,214,138,.2)}
.sent-badge.BEARISH{color:var(--red);background:var(--red-d);border:1px solid rgba(240,70,104,.2)}
.sent-badge.NEUTRAL{color:var(--gold);background:var(--gold-d);border:1px solid rgba(245,183,49,.2)}
.sent-conf{font:12px var(--mono);color:var(--t2)}
.sent-conf b{font-size:20px;color:var(--t1)}
.sent-risk{font:11px var(--mono);padding:4px 10px;border-radius:6px;margin-top:4px}
.sent-risk.LOW{color:var(--green);background:var(--green-d)}
.sent-risk.MODERATE{color:var(--gold);background:var(--gold-d)}
.sent-risk.HIGH{color:var(--red);background:var(--red-d)}
.sent-risk.EXTREME{color:#fff;background:var(--red);font-weight:700}

.sent-details{display:flex;flex-direction:column;gap:12px}
.sent-summary{font:14px var(--sans);color:var(--t1);line-height:1.5}
.sent-outlook{font:13px var(--sans);color:var(--t2);line-height:1.5;font-style:italic}
.sent-drivers{display:flex;flex-wrap:wrap;gap:6px}
.sent-driver{font:11px var(--mono);padding:4px 10px;border-radius:6px;background:rgba(255,255,255,.04);border:1px solid var(--border);color:var(--t2)}

/* News */
.news-list{display:flex;flex-direction:column;gap:8px;margin-top:8px}
.news-item{display:flex;align-items:center;gap:10px;padding:8px 12px;border-radius:8px;background:rgba(255,255,255,.02);border:1px solid var(--border)}
.news-impact{font:700 9px var(--mono);padding:3px 8px;border-radius:4px;min-width:42px;text-align:center}
.news-impact.HIGH{color:var(--red);background:var(--red-d)}
.news-impact.MEDIUM{color:var(--gold);background:var(--gold-d)}
.news-impact.LOW{color:var(--t2);background:rgba(255,255,255,.04)}
.news-title{font:12px var(--sans);color:var(--t1);flex:1}
.news-bias{font:10px var(--mono);padding:2px 8px;border-radius:4px}
.news-bias.BULLISH{color:var(--green)}.news-bias.BEARISH{color:var(--red)}.news-bias.NEUTRAL{color:var(--t2)}
.sent-age{font:10px var(--mono);color:var(--t3);margin-top:4px}
.sent-loading{font:13px var(--mono);color:var(--t3);text-align:center;padding:40px}

/* Data grid */
.dg{grid-column:span 3;display:grid;grid-template-columns:repeat(4,1fr);gap:12px}
.dc{background:var(--card);border:1px solid var(--border);border-radius:10px;padding:14px 16px}
.dcl{font:10px var(--mono);text-transform:uppercase;letter-spacing:1px;color:var(--t3);margin-bottom:6px}
.dcv{font:600 16px var(--mono)}

/* Sessions */
.sb{grid-column:span 3;display:flex;gap:12px}
.si{flex:1;display:flex;align-items:center;gap:10px;padding:14px 18px;border-radius:12px;background:var(--card);border:1px solid var(--border);font:12px var(--mono)}
.sd{width:10px;height:10px;border-radius:50%;background:var(--t3);flex-shrink:0}
.sd.on{background:var(--green);box-shadow:0 0 10px rgba(34,214,138,.3)}
.sl{color:var(--t2)}.st{color:var(--t1);margin-left:auto}

.nd{display:flex;flex-direction:column;align-items:center;justify-content:center;padding:60px;color:var(--t3);font:14px var(--mono);grid-column:span 3}
.nd .sp{width:24px;height:24px;border:2px solid var(--border);border-top-color:var(--gold);border-radius:50%;animation:spin .8s linear infinite;margin-bottom:14px}
@keyframes spin{to{transform:rotate(360deg)}}

@media(max-width:768px){
.g{grid-template-columns:1fr}
.pc,.cc,.sb,.dg,.sent-card{grid-column:span 1}
.dg{grid-template-columns:repeat(2,1fr)}
.pv{font-size:36px}
.pc{flex-direction:column;align-items:flex-start;gap:12px}
.pm{text-align:left}
.cw{height:280px}
.sent-card{grid-template-columns:1fr}
}
</style>
</head>
<body>
<div class='c'>
<div class='hdr'>
    <div class='hdr-l'><div class='logo'>SS</div><h1>Spike<span>Surfer</span></h1></div>
    <div class='conn'><div class='dot' id='sDot'></div><span id='sTxt'>Connecting...</span></div>
</div>
<div class='g' id='main'>
    <div class='nd'><div class='sp'></div>Waiting for market data...</div>
</div>
</div>

<script>
const P=2000;let lM=null,lD=null,dr=false,lC=null,lL=[],lV=[],lMax=30,cTF='m5',lastCd=[];

function init(){
document.getElementById('main').innerHTML=`
<div class='cd pc'><div><div class='sym'>XAUUSD</div><div class='pv' id='pr'>--</div><div class='pch' id='ch'>--</div></div><div class='pm'><div>BID <span id='bi'>--</span></div><div>ASK <span id='ak'>--</span></div><div>SPREAD <span id='sp'>--</span></div></div></div>
<div class='cd'><div class='cl'>Session</div><div class='sv' id='se'>--</div><div class='ss' id='ti'>--</div></div>

<div class='cc cd' style='position:relative'><div class='ch'><div class='cl' style='margin:0'>Candlesticks <span id='cN' style='color:var(--t3);font-weight:400'></span></div><div class='tabs' id='cT'><button class='tab' onclick='sTF(""m1"",this)'>M1</button><button class='tab on' onclick='sTF(""m5"",this)'>M5</button><button class='tab' onclick='sTF(""m15"",this)'>M15</button></div></div><div class='cw' id='cW'><canvas id='cC'></canvas><div class='ctt' id='cTt'></div></div></div>

<div class='cc cd'><div class='ch'><div class='cl' style='margin:0'>Live Price</div><div class='tabs'><button class='tab on' onclick='sLM(30,this)'>1M</button><button class='tab' onclick='sLM(150,this)'>5M</button><button class='tab' onclick='sLM(450,this)'>15M</button></div></div><div class='lw'><canvas id='lC'></canvas></div></div>

<div class='sent-card cd' id='sentCard'><div class='sent-loading'>Waiting for Gemini analysis...</div></div>

<div class='dg'>
<div class='dc'><div class='dcl'>1h Change</div><div class='dcv' id='c1'>--</div></div>
<div class='dc'><div class='dcl'>1h %</div><div class='dcv' id='c1p'>--</div></div>
<div class='dc'><div class='dcl'>High (session)</div><div class='dcv' id='sH'>--</div></div>
<div class='dc'><div class='dcl'>Low (session)</div><div class='dcv' id='sL'>--</div></div>
</div>

<div class='sb'>
<div class='si'><div class='sd' id='dA'></div><span class='sl'>Asia</span><span class='st'>00:00-07:00</span></div>
<div class='si'><div class='sd' id='dL'></div><span class='sl'>London</span><span class='st'>07:00-16:00</span></div>
<div class='si'><div class='sd' id='dN'></div><span class='sl'>New York</span><span class='st'>12:00-21:00</span></div>
</div>
`;
iLC();hov();
}

function iLC(){
const x=document.getElementById('lC'),g=x.getContext('2d'),gr=g.createLinearGradient(0,0,0,180);
gr.addColorStop(0,'rgba(245,183,49,.15)');gr.addColorStop(1,'rgba(245,183,49,0)');
lC=new Chart(x,{type:'line',data:{labels:lL,datasets:[{data:lV,borderColor:'#f5b731',borderWidth:2,backgroundColor:gr,fill:true,tension:.3,pointRadius:0,pointHoverRadius:4,pointHoverBackgroundColor:'#f5b731'}]},options:{responsive:true,maintainAspectRatio:false,interaction:{intersect:false,mode:'index'},plugins:{legend:{display:false},tooltip:{backgroundColor:'#1a1d2a',titleColor:'#6b7084',bodyColor:'#e8eaf0',titleFont:{family:'JetBrains Mono',size:11},bodyFont:{family:'JetBrains Mono',size:13,weight:'600'},padding:12,cornerRadius:8,borderColor:'#2a2d3a',borderWidth:1,displayColors:false,callbacks:{label:c=>c.parsed.y.toFixed(2)}}},scales:{x:{grid:{color:'rgba(255,255,255,.03)',drawBorder:false},ticks:{color:'#3d4156',font:{family:'JetBrains Mono',size:10},maxTicksLimit:8,maxRotation:0}},y:{position:'right',grid:{color:'rgba(255,255,255,.03)',drawBorder:false},ticks:{color:'#3d4156',font:{family:'JetBrains Mono',size:10},callback:v=>v.toFixed(1)}}}}});
}
function sLM(m,e){lMax=m;while(lL.length>lMax){lL.shift();lV.shift()}if(lC)lC.update('none');document.querySelectorAll('.cc:nth-of-type(2) .tab').forEach(t=>t.classList.remove('on'));e.classList.add('on')}
function sTF(t,e){cTF=t;document.querySelectorAll('#cT .tab').forEach(b=>b.classList.remove('on'));e.classList.add('on');fC()}
async function fC(){try{const r=await fetch('/api/market/candles/'+cTF);if(r.ok){lastCd=await r.json();dC(lastCd)}}catch{}}

function hov(){
const w=document.getElementById('cW'),t=document.getElementById('cTt'),cv=document.getElementById('cC');
if(!w||!t||!cv)return;
cv.addEventListener('mousemove',e=>{if(!lastCd||!lastCd.length){t.style.display='none';return}const r=cv.getBoundingClientRect(),x=e.clientX-r.left,p={l:10,r:60},cW=r.width-p.l-p.r,tw=cW/lastCd.length,i=Math.floor((x-p.l)/tw);if(i<0||i>=lastCd.length){t.style.display='none';return}const c=lastCd[i],g=c.close>=c.open,a=g?'\u25B2':'\u25BC',co=g?'var(--green)':'var(--red)',ch=((c.close-c.open)/c.open*100).toFixed(3),tm=c.time?c.time.slice(11,16):'';t.innerHTML=`<div style='color:var(--t3);margin-bottom:4px'>${cTF.toUpperCase()} ${tm}</div><div>O: ${c.open.toFixed(2)}</div><div>H: <span style='color:var(--green)'>${c.high.toFixed(2)}</span></div><div>L: <span style='color:var(--red)'>${c.low.toFixed(2)}</span></div><div>C: ${c.close.toFixed(2)}</div><div style='color:${co};margin-top:4px'>${a} ${ch}%</div>`;t.style.display='block';let tx=e.clientX-r.left+16,ty=e.clientY-r.top-40;if(tx+160>r.width)tx=e.clientX-r.left-170;if(ty<0)ty=10;t.style.left=tx+'px';t.style.top=ty+'px'});
cv.addEventListener('mouseleave',()=>{t.style.display='none'});
}

function dC(cd){
const cv=document.getElementById('cC');if(!cv)return;
const dp=window.devicePixelRatio||1,rc=cv.parentElement.getBoundingClientRect();
cv.width=rc.width*dp;cv.height=rc.height*dp;
const x=cv.getContext('2d');x.scale(dp,dp);
const W=rc.width,H=rc.height;x.clearRect(0,0,W,H);
if(!cd||!cd.length){x.fillStyle='#3d4156';x.font='13px JetBrains Mono';x.textAlign='center';x.fillText('Waiting for '+cTF.toUpperCase()+' candles...',W/2,H/2);return}
const pd={t:20,b:40,l:10,r:60},cW=W-pd.l-pd.r,cH=H-pd.t-pd.b;
let mn=Infinity,mx=-Infinity;cd.forEach(c=>{if(c.low<mn)mn=c.low;if(c.high>mx)mx=c.high});
const pr=mx-mn||1,pm=pr*.1;mn-=pm;mx+=pm;const tr=mx-mn;
function pY(p){return pd.t+cH-((p-mn)/tr)*cH}
const gl=6;x.strokeStyle='rgba(255,255,255,.04)';x.lineWidth=1;x.setLineDash([]);
for(let i=0;i<=gl;i++){const y=pd.t+(cH/gl)*i;x.beginPath();x.moveTo(pd.l,y);x.lineTo(W-pd.r,y);x.stroke();x.fillStyle='#3d4156';x.font='10px JetBrains Mono';x.textAlign='left';x.fillText((mx-(tr/gl)*i).toFixed(1),W-pd.r+8,y+4)}
const sp=3,tw=cW/cd.length,bW=Math.max(tw-sp,2),wW=Math.max(1,bW*.15),le=cd.length>30?6:cd.length>15?3:2;
cd.forEach((c,i)=>{const cx=pd.l+i*tw+tw/2,ig=c.close>=c.open,co=ig?'#22d68a':'#f04668',hY=pY(c.high),lY=pY(c.low),oY=pY(c.open),cY=pY(c.close),bt=Math.min(oY,cY),bH=Math.max(Math.abs(cY-oY),1);
x.strokeStyle=co;x.lineWidth=wW;x.beginPath();x.moveTo(cx,hY);x.lineTo(cx,lY);x.stroke();
x.fillStyle=co;if(ig){x.globalAlpha=.25;x.fillRect(cx-bW/2,bt,bW,bH);x.globalAlpha=1;x.strokeStyle=co;x.lineWidth=1.5;x.strokeRect(cx-bW/2,bt,bW,bH)}else{x.globalAlpha=.9;x.fillRect(cx-bW/2,bt,bW,bH);x.globalAlpha=1}
if(i%le===0){x.fillStyle='#3d4156';x.font='10px JetBrains Mono';x.textAlign='center';x.fillText(c.time?c.time.slice(11,16):'',cx,H-pd.b+18)}});
if(cd.length>0){const l=cd[cd.length-1],y=pY(l.close);x.setLineDash([4,4]);x.strokeStyle=l.close>=l.open?'#22d68a88':'#f0466888';x.lineWidth=1;x.beginPath();x.moveTo(pd.l,y);x.lineTo(W-pd.r,y);x.stroke();x.setLineDash([]);x.fillStyle=l.close>=l.open?'#22d68a':'#f04668';x.beginPath();x.roundRect(W-pd.r+2,y-10,56,20,4);x.fill();x.fillStyle='#000';x.font='bold 11px JetBrains Mono';x.textAlign='center';x.fillText(l.close.toFixed(2),W-pd.r+30,y+4)}
document.getElementById('cN').textContent='('+cd.length+')';
}

function renderSentiment(s){
const el=document.getElementById('sentCard');
if(!s){el.innerHTML=`<div class='sent-loading'>Waiting for Gemini analysis...</div>`;return}
const age=Math.round((Date.now()-new Date(s.timestamp).getTime())/1000);
const ageStr=age<60?age+'s ago':Math.round(age/60)+'m ago';
const newsHtml=s.newsEvents&&s.newsEvents.length?s.newsEvents.map(n=>`<div class='news-item'><span class='news-impact ${n.impact}'>${n.impact}</span><span class='news-title'>${n.title}</span><span class='news-bias ${n.bias}'>${n.bias}</span></div>`).join(''):'<div style=""color:var(--t3);font:12px var(--mono)"">No significant events</div>';
const driversHtml=s.keyDrivers&&s.keyDrivers.length?s.keyDrivers.map(d=>`<span class='sent-driver'>${d}</span>`).join(''):'';

el.innerHTML=`
<div class='sent-gauge'>
    <div class='cl' style='margin:0'>AI Sentiment</div>
    <div class='sent-badge ${s.sentiment}'>${s.sentiment}</div>
    <div class='sent-conf'>Confidence: <b>${s.confidence}%</b></div>
    <div class='sent-risk ${s.riskLevel}'>Risk: ${s.riskLevel}</div>
    <div class='sent-age'>${ageStr}</div>
</div>
<div class='sent-details'>
    <div class='sent-summary'>${s.summary}</div>
    <div class='sent-outlook'>${s.shortTermOutlook}</div>
    <div class='sent-drivers'>${driversHtml}</div>
    <div class='cl' style='margin-top:8px;margin-bottom:0'>News & Events</div>
    <div class='news-list'>${newsHtml}</div>
</div>`;
}

function gAS(h){const s=[];if(h>=0&&h<7)s.push('A');if(h>=7&&h<16)s.push('L');if(h>=12&&h<21)s.push('N');return s}
let sH=-Infinity,sLo=Infinity;

async function upd(){
try{
const[pR,cR,sR]=await Promise.all([fetch('/api/market/xauusd'),fetch('/api/market/candles/'+cTF),fetch('/api/market/sentiment')]);
if(!pR.ok){const d=document.getElementById('sDot'),t=document.getElementById('sTxt');if(d)d.classList.remove('on');if(t)t.textContent='No data';return}
const d=await pR.json(),cd=cR.ok?await cR.json():[],sent=sR.ok?await sR.json():null;
lastCd=cd;
if(!dr){dr=true;init()}
document.getElementById('sDot').classList.add('on');document.getElementById('sTxt').textContent='LIVE';
const dir=lM!==null?(d.mid>lM?'up':d.mid<lM?'down':lD):null;lD=dir;lM=d.mid;
const pe=document.getElementById('pr');pe.textContent=d.mid.toFixed(2);pe.className='pv'+(dir?' '+dir:'');
const pc=d.change1hPercent.toFixed(3),iu=d.change1hPercent>=0,ce=document.getElementById('ch');
ce.textContent=(iu?'\u25B2 +':'\u25BC ')+pc+'%';ce.className='pch '+(iu?'up':'down');
document.getElementById('bi').textContent=d.bid.toFixed(2);document.getElementById('ak').textContent=d.ask.toFixed(2);document.getElementById('sp').textContent=d.spread.toFixed(1)+' pts';
const st=new Date(d.serverTimeUtc),h=st.getUTCHours(),as=gAS(h);
document.getElementById('se').textContent=as.length===0?'Closed':as.map(s=>s==='A'?'Asia':s==='L'?'London':'New York').join(' / ');
document.getElementById('ti').textContent=st.toISOString().slice(11,19)+' UTC';
[['A','dA'],['L','dL'],['N','dN']].forEach(([k,id])=>{const e=document.getElementById(id);if(e){as.includes(k)?e.classList.add('on'):e.classList.remove('on')}});
const c1=d.change1h;document.getElementById('c1').textContent=(c1>=0?'+':'')+c1.toFixed(2);document.getElementById('c1').style.color=c1>=0?'var(--green)':'var(--red)';
document.getElementById('c1p').textContent=(d.change1hPercent>=0?'+':'')+d.change1hPercent.toFixed(3)+'%';document.getElementById('c1p').style.color=d.change1hPercent>=0?'var(--green)':'var(--red)';
if(d.mid>sH)sH=d.mid;if(d.mid<sLo)sLo=d.mid;
document.getElementById('sH').textContent=sH.toFixed(2);document.getElementById('sL').textContent=sLo.toFixed(2);
dC(cd);
renderSentiment(sent);
const now=new Date();lL.push(now.toLocaleTimeString([],{hour:'2-digit',minute:'2-digit',second:'2-digit'}));lV.push(d.mid);while(lL.length>lMax){lL.shift();lV.shift()}if(lC)lC.update('none');
}catch{const d=document.getElementById('sDot'),t=document.getElementById('sTxt');if(d)d.classList.remove('on');if(t)t.textContent='Disconnected'}}

setInterval(upd,P);upd();
window.addEventListener('resize',()=>{if(dr&&lastCd.length)dC(lastCd)});
</script>
</body>
</html>
";
        return Content(html, "text/html");
    }
}