// api/cabin.js — DEBUG VERSION

const MODEL_CODES = {
  "9a": "0", "9s": "1", "94": "2", "9f": "3", "9b": "4", "9c": "5",
  "vs": "8", "vf": "7", "v4": "8", "va": "9",
  "ws": "10", "e": "11", "flipper": "12",
  "nb": "13", "db": "14", "nv": "15", "mv2": "16",
  "icp": "17", "glasscontainer": "18", "qb": "19", "qp": "20"
};

const GLASS_THICKNESS_CODES = { "6mm": "2", "6/8mm": "3", "8mm": "4" };
const GLASS_FINISH_CODES = { "fume": "4" };
const DRAW_NUMBER_BY_MODEL = { "vs": "14" };
const EXTRAS_CODES = {
  "safekid": "2exp",
  "nano": "NNexp",
  "bronzeclean": "BCexp"
};

function interpretRequest(text) {
  text = text.toLowerCase();

  const model_key = Object.keys(MODEL_CODES).find(k => text.includes(k));
  const glass_key = Object.keys(GLASS_THICKNESS_CODES).find(k => text.includes(k));
  const finish_key = Object.keys(GLASS_FINISH_CODES).find(k => text.includes(k));

  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width = dimMatch ? parseInt(dimMatch[1], 10) : null;
  const height = dimMatch ? parseInt(dimMatch[2], 10) : null;

  const extras = Object.keys(EXTRAS_CODES).filter(k => text.includes(k));

  console.log("🔍 INTERPRETED:", { model_key, glass_key, finish_key, width, height, extras });
  return { model_key, glass_key, finish_key, width, height, extras };
}

function generateCabinDetailCode(req) {
  const modelCode = MODEL_CODES[req.model_key];
  const thicknessCode = GLASS_THICKNESS_CODES[req.glass_key] || "2";
  const glassFinishCode = GLASS_FINISH_CODES[req.finish_key] || "0";
  const drawNumber = DRAW_NUMBER_BY_MODEL[req.model_key] || "1";

  const widthMM = (req.width || 100) * 10;
  const heightMM = (req.height || 190) * 10;

  const extrasCode = req.extras.length
    ? (EXTRAS_CODES[req.extras[0]] || "nullexp")
    : "nullexp";

  const result = `${modelCode}-0-${thicknessCode}-${glassFinishCode}-${drawNumber}-0-0-` +
                 `${widthMM}-${heightMM}-${extrasCode}nullexpnullexpnullexxcccnullcccnullcccprtprt`;

  console.log("🧪 Generated code:", result);
  return result;
}

export default function handler(req, res) {
  if (req.method !== 'POST') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  const { text } = req.body || {};
  console.log("📨 Incoming text:", text);

  if (!text) {
    return res.status(400).json({ error: 'Missing text field.' });
  }

  try {
    const parsed = interpretRequest(text);

    if (!parsed.model_key || !parsed.glass_key) {
      console.warn("⚠️ Λείπει model_key ή glass_key");
      return res.status(400).json({
        error: "Λείπει το μοντέλο ή το πάχος γυαλιού. Π.χ. 'VS 120x190 φιμέ 8mm SafeKid'"
      });
    }

    const cabinCode = generateCabinDetailCode(parsed);
    const cabinURL = `https://www.bronzeapp.eu/AssembleCabinLink/${cabinCode}`;

    return res.status(200).json({
      cabin_url: cabinURL,
      internal_debug_code: `${parsed.model_key}-${parsed.width}-${parsed.height}-${parsed.glass_key}-${parsed.extras.join(",")}`,
      details: parsed
    });
  } catch (e) {
    console.error("⛔ Internal error:", e);
    return res.status(500).json({ error: e?.message || "Άγνωστο σφάλμα." });
  }
}

