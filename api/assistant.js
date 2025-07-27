// api/assistant.js

const MODEL_CODES = {
  "γωνιακή": "0",
  "συρόμενη": "1",
  "διπλή συρόμενη": "2",
  "σταθερή": "3",
  "ανοιγόμενη": "4",
  "ημικυκλική": "5",
  "inox": "6"
};

const FINISH_CODES = {
  "μαύρο ματ": "2",
  "λευκό ματ": "3",
  "ματ": "1",
  "γυαλιστερό": "0",
  "αντικέ": "4"
};

const GLASS_CODES = {
  "σατινέ": "1",
  "διάφανο": "0",
  "φιμέ": "2",
  "σεριγραφία": "3"
};

const EXTRAS_CODES = {
  "SafeKid": "SK",
  "BronzeClean": "BC",
  "Nano": "NN"
};

function interpretRequest(text) {
  text = text.toLowerCase();
  
  const model = Object.keys(MODEL_CODES).find(k => text.includes(k)) || "συρόμενη";
  const finish = Object.keys(FINISH_CODES).find(k => text.includes(k)) || "γυαλιστερό";
  const glass = Object.keys(GLASS_CODES).find(k => text.includes(k)) || "διάφανο";

  const extras = Object.keys(EXTRAS_CODES).filter(k => text.includes(k.toLowerCase()));

  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width = dimMatch ? parseInt(dimMatch[1]) : 100;
  const height = dimMatch ? parseInt(dimMatch[2]) : 190;

  return { model, width, height, finish, glass, extras };
}

function generateCabinCode(req) {
  const modelCode = MODEL_CODES[req.model];
  const finishCode = FINISH_CODES[req.finish];
  const glassCode = GLASS_CODES[req.glass];
  const extrasCode = req.extras.map(e => EXTRAS_CODES[e]).join('');

  return `${modelCode}-${finishCode}-${glassCode}-${req.width}-${req.height}-${extrasCode}`;
}

export default async function handler(req, res) {
  const { text } = req.body;

  if (!text) {
    return res.status(400).json({ error: "Λείπει το πεδίο 'text' από το request." });
  }

  const parsedRequest = interpretRequest(text);
  const cabin_code = generateCabinCode(parsedRequest);
  const cabin_url = `https://www.bronzeapp.eu/AssembleCabinLink/${cabin_code}`;

  return res.status(200).json({
    cabin_code,
    cabin_url,
    details: parsedRequest
  });
}

