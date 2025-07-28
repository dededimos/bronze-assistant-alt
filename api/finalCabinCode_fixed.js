// api/finalCabinCode_fixed.js
// Complete, validated version that supports all models and glass thickness codes

const MODEL_CODES = {
  "9a": "0",
  "9s": "1",
  "94": "2",
  "9f": "3",
  "9b": "4",
  "9c": "5",
  "vs": "6",
  "vf": "7",
  "v4": "8",
  "va": "9",
  "ws": "10",
  "e": "11",
  "flipper": "12",
  "nb": "13",
  "db": "14",
  "nv": "15",
  "mv2": "16",
  "icp": "17",
  "glasscontainer": "18",
  "qb": "19",
  "qp": "20"
};

const GLASS_THICKNESS_CODES = {
  "6mm": "2",
  "6/8mm": "3",
  "8mm": "4"
};

const EXTRAS_CODES = {
  "safekid": "SK",
  "bronzeclean": "BC",
  "nano": "NN"
};

function interpretRequest(text) {
  text = text.toLowerCase();

  // Model
  const model_key = Object.keys(MODEL_CODES).find(k => text.includes(k));

  // Glass thickness
  const glass_key = Object.keys(GLASS_THICKNESS_CODES).find(k => text.includes(k));

  // Dimensions pattern (e.g. 100x190 or 100×190)
  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width = dimMatch ? parseInt(dimMatch[1], 10) : null;
  const height = dimMatch ? parseInt(dimMatch[2], 10) : null;

  // Extras
  const extras = Object.keys(EXTRAS_CODES)
    .filter(k => text.includes(k))
    .map(k => EXTRAS_CODES[k]);

  return {
    model_key,
    glass_key,
    width,
    height,
    extras
  };
}

function generateCabinDetailCode(req) {
  const modelCode = MODEL_CODES[req.model_key];
  const thicknessCode = GLASS_THICKNESS_CODES[req.glass_key] || "2"; // default 6 mm

  const widthMM = (req.width || 100) * 10;   // convert to mm
  const heightMM = (req.height || 190) * 10; // convert to mm

  const extrasCode = req.extras.length ? req.extras.join("") : "nullexp";

  return `${modelCode}-${thicknessCode}-0-1-1-0-1-${widthMM}-${heightMM}-${extrasCode}nullexpnullexpnullexxcccnullcccprtprt`;
}

export default async function handler(req, res) {
  const { text } = req.body;

  if (!text) {
    return res.status(400).json({ error: "Λείπει το πεδίο 'text' από το αίτημα." });
  }

  const parsed = interpretRequest(text);

  if (!parsed.model_key) {
    return res.status(400).json({ error: "Δεν αναγνωρίστηκε μοντέλο καμπίνας. Π.χ. 9S, VS, NV κ.λπ." });
  }

  const cabin_code = generateCabinDetailCode(parsed);
  const cabin_url = `https://www.bronzeapp.eu/AssembleCabinLink/${cabin_code}`;

  return res.status(200).json({
    cabin_url,
    internal_debug_code: `${parsed.model_key}-${parsed.width || "default"}-${parsed.height || "default"}-${parsed.glass_key || "6mm"}-${parsed.extras.join("-") || "noextras"}`,
    details: parsed
  });
}
