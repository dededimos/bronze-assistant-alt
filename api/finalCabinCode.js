// api/finalCabinCode.js

const MODEL_CODES = {
  "9a": "0",
  "9s": "1",
  "94": "2",
  "9f": "3",
  "9b": "4",
  "9c": "5",
  "vs": "6"
};

const FINISH_CODES = {
  "polished": "0",
  "brushed": "1",
  "blackmat": "2",
  "whitemat": "3",
  "bronze": "4"
};

const GLASS_CODES = {
  "transparent": "0",
  "satin": "1",
  "fume": "2",
  "serigraphy": "3"
};

const EXTRAS_CODES = {
  "safekid": "SK",
  "bronzeclean": "BC",
  "nano": "NN"
};

function interpretRequest(text) {
  text = text.toLowerCase();

  const model_key = Object.keys(MODEL_CODES).find(k => text.includes(k)) || "9s";
  const finish_key = Object.keys(FINISH_CODES).find(k => text.includes(k)) || "polished";
  const glass_key = Object.keys(GLASS_CODES).find(k => text.includes(k)) || "transparent";

  const extras = Object.keys(EXTRAS_CODES).filter(k => text.includes(k));

  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width = dimMatch ? parseInt(dimMatch[1]) : 100;
  const height = dimMatch ? parseInt(dimMatch[2]) : 190;

  return { model_key, width, height, finish_key, glass_key, extras };
}

function generateCabinDetailedCode(req) {
  const model = MODEL_CODES[req.model_key];
  const finish = FINISH_CODES[req.finish_key];
  const glass = GLASS_CODES[req.glass_key];
  const width = req.width * 10;
  const height = req.height * 10;
  const extras_part = req.extras.length > 0 ? req.extras.map(e => EXTRAS_CODES[e]).join("") : "nullexp";

  return `${model}-${finish}-${glass}-1-1-0-1-${width}-${height}-${extras_part}nullexpnullexpnullexxcccnullcccnullcccprtprt`;
}

function generateDebugCode(req) {
  const extras_debug = req.extras.join("-");
  return `${req.model_key}${req.width}-${req.finish_key}-${req.glass_key}-${req.height}${extras_debug ? "-" + extras_debug : ""}`;
}

export default async function handler(req, res) {
  const { text } = req.body;

  if (!text) {
    return res.status(400).json({ error: "Λείπει το πεδίο 'text' από το request." });
  }

  const parsedRequest = interpretRequest(text);
  const detailed_code = generateCabinDetailedCode(parsedRequest);
  const cabin_url = `https://www.bronzeapp.eu/AssembleCabinLink/${detailed_code}`;

  const debug_code = generateDebugCode(parsedRequest);

  return res.status(200).json({
    cabin_url,
    internal_debug_code: debug_code,
    details: parsedRequest
  });
}

