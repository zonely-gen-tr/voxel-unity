import fs from "node:fs";
import path from "node:path";

const inputDir = path.resolve(process.argv[2] || "Build/Web");
const outputFile = path.resolve(process.argv[3] || "Build/Single/zonely-voxel-engine.html");

if (!fs.existsSync(inputDir)) {
  console.error("Build klasörü bulunamadı:", inputDir);
  process.exit(1);
}

const walk = (dir) => fs.readdirSync(dir, {withFileTypes:true}).flatMap(e => {
  const p = path.join(dir, e.name);
  return e.isDirectory() ? walk(p) : [p];
});

const all = walk(inputDir);
const one = (rx) => all.find(f => rx.test(path.basename(f)));

const loader = one(/\.loader\.js$/i);
const framework = one(/\.framework\.js$/i);
const wasm = one(/\.wasm$/i);
const data = one(/\.data$/i);
const symbols = one(/\.symbols\.json$/i);
const mem = one(/\.mem$/i);

for (const [name, file] of Object.entries({loader,framework,wasm,data})) {
  if (!file) {
    console.error(`Gerekli Unity dosyası yok: ${name}`);
    console.error("Compression Format = Disabled kullan.");
    process.exit(2);
  }
}

const b64 = file => fs.readFileSync(file).toString("base64");
const jsText = file => fs.readFileSync(file, "utf8");

const embedded = {
  framework: b64(framework),
  wasm: b64(wasm),
  data: b64(data),
  symbols: symbols ? b64(symbols) : null,
  mem: mem ? b64(mem) : null
};

const loaderJs = jsText(loader)
  .replace(/<\/script/gi, "<\\/script");

const html = `<!doctype html>
<html lang="tr">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
<title>Zonely Voxel Game Engine</title>
<style>
html,body{margin:0;width:100%;height:100%;overflow:hidden;background:#0b1118}
#unity-canvas{width:100%;height:100%;display:block;background:#111}
#loading{position:fixed;inset:0;display:grid;place-items:center;color:#fff;font:600 14px system-ui;background:#0b1118;z-index:3}
.card{width:min(420px,80vw)}
.bar{height:8px;border-radius:999px;background:#ffffff18;overflow:hidden;margin-top:12px}
.fill{height:100%;width:0;background:#fff;transition:width .12s}
.small{opacity:.65;font-size:12px;margin-top:8px}
</style>
</head>
<body>
<canvas id="unity-canvas" tabindex="-1"></canvas>
<div id="loading"><div class="card">
  <div id="label">Unity yükleniyor…</div>
  <div class="bar"><div class="fill" id="fill"></div></div>
  <div class="small">Tek HTML • WebAssembly</div>
</div></div>
<script>
const EMBEDDED=${JSON.stringify(embedded)};

function bytesFromBase64(s){
  const raw=atob(s), out=new Uint8Array(raw.length);
  for(let i=0;i<raw.length;i++) out[i]=raw.charCodeAt(i);
  return out;
}
function blobUrl(b64,type){
  return URL.createObjectURL(new Blob([bytesFromBase64(b64)],{type}));
}

const urls={
  frameworkUrl:blobUrl(EMBEDDED.framework,"text/javascript"),
  codeUrl:blobUrl(EMBEDDED.wasm,"application/wasm"),
  dataUrl:blobUrl(EMBEDDED.data,"application/octet-stream")
};
if(EMBEDDED.symbols) urls.symbolsUrl=blobUrl(EMBEDDED.symbols,"application/json");
if(EMBEDDED.mem) urls.memoryUrl=blobUrl(EMBEDDED.mem,"application/octet-stream");

${loaderJs}

const canvas=document.querySelector("#unity-canvas");
const label=document.querySelector("#label");
const fill=document.querySelector("#fill");

const config={
  dataUrl:urls.dataUrl,
  frameworkUrl:urls.frameworkUrl,
  codeUrl:urls.codeUrl,
  streamingAssetsUrl:"",
  companyName:"ZonelyVoxelEngine",
  productName:"Zonely Voxel Game Engine",
  productVersion:"0.1.0-dev"
};
if(urls.symbolsUrl) config.symbolsUrl=urls.symbolsUrl;
if(urls.memoryUrl) config.memoryUrl=urls.memoryUrl;

createUnityInstance(canvas,config,(p)=>{
  fill.style.width=(p*100).toFixed(0)+"%";
  label.textContent="Unity yükleniyor… "+Math.round(p*100)+"%";
}).then((instance)=>{
  document.querySelector("#loading").style.display="none";
  window.unityInstance=instance;
}).catch((err)=>{
  label.textContent="Unity başlatılamadı";
  console.error(err);
  alert(err);
});
</script>
</body>
</html>`;

fs.mkdirSync(path.dirname(outputFile), {recursive:true});
fs.writeFileSync(outputFile, html);
console.log("Single HTML oluşturuldu:", outputFile);
console.log("Boyut:", (fs.statSync(outputFile).size/1024/1024).toFixed(2), "MB");
