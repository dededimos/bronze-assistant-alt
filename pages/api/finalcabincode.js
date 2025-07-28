// api/finalCabinCode.js (v8 logic)
// VS 140×200 fume 8 mm, BronzeClean – corrected pattern

const MODEL_CODES = {
  "9a": "0", "9s": "1", "94": "2", "9f": "3", "9b": "4", "9c": "5",
  "vs": "8", // VS
  "vf": "7", "v4": "8", "va": "9",
  "ws": "10", "e": "11", "flipper": "12",
  "nb": "13", "db": "14", "nv": "15", "mv2": "16",
  "icp": "17", "glasscontainer": "18", "qb": "19", "qp": "20"
};

const GLASS_THICKNESS_CODES = { "6mm": "2", "6/8mm": "3", "8mm": "4" };
const GLASS_FINISH_CODES   = { "fume": "4" };   // φιμέ
const DRAW_NUMBER_BY_MODEL = { "vs": "14" };    // VS → 14

// Extras που εμφανίζονται στο URL
const EXTRAS_CODES = { "safekid": "2exp" };     // BronzeClean δεν μπαίνει

function interpretRequest(text) {
  text = text.toLowerCase();

  const model_key  = Object.keys(MODEL_CODES).find(k => text.includes(k));
  const glass_key  = Object.keys(GLASS_THICKNESS_CODES).find(k => text.includes(k));
  const finish_key = Object.keys(GLASS_FINISH_CODES).find(k => text.includes(k));

  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width  = dimMatch ? parseInt(dimMatch[1], 10) : null;
  const height = dimMatch ? parseInt(dimMatch[2], 10) : null;

  const extras = Object.keys(EXTRAS_CODES).filter(k => text.includes(k));

  return { model_key, glass_key, finish_key, width, height, extras };
}

export function generateCabinDetailCode(req) {
  const modelCode       = MODEL_CODES[req.model_key];
  const thicknessCode   = GLASS_THICKNESS_CODES[req.glass_key] || "2";
  const glassFinishCode = GLASS_FINISH_CODES[req.finish_key]   || "0";
  const drawNumber      = DRAW_NUMBER_BY_MODEL[req.model_key]  || "1";

  const widthMM  = (req.width  || 100) * 10;
  const heightMM = (req.height || 190) * 10;

  const extrasCode = req.extras.length
    ? (EXTRAS_CODES[req.extras[0]] || "nullexp")
    : "nullexp";

  return `${modelCode}-0-${thicknessCode}-${glassFinishCode}-${drawNumber}-0-0-` +
         `${widthMM}-${heightMM}-${extrasCode}nullexpnullexxcccnullcccnullcccprtprt`;
}
