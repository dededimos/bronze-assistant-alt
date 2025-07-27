// api/assistant.js

const MODEL_MAP = {
  "γωνιακή": "9A",
  "συρόμενη": "9S",
  "διπλή συρόμενη": "94",
  "σταθερή": "9F",
  "ανοιγόμενη": "9B",
  "ημικυκλική": "9C",
  "inox": "VS",
};

const FINISH_MAP = {
  "μαύρο ματ": "B",
  "λευκό ματ": "W",
  "ματ": "M",
  "γυαλιστερό": "G",
  "αντικέ": "A",
};

const GLASS_MAP = {
  "σατινέ": "S",
  "διάφανο": "T",
  "φιμέ": "F",
  "σεριγραφία": "R",
};

const EXTRAS_MAP = {
  "safekid": "SafeKid",
  "bronzeclean": "BronzeClean",
  "nano": "BronzeNano",
};

function interpretRequest(text) {
  text = text.toLowerCase();
  
  const model_code = Object.entries(MODEL_MAP).find(([k]) => text.includes(k))?.[1] || "9S";
  const finish = Object.entries(FINISH_MAP).find(([k]) => text.includes(k))?.[1] || "G";
  const glass = Object.entries(GLASS_MAP).find(([k]) => text.includes(k))?.[1] || "T";
  const extras = Object.entries(EXTRAS_MAP)
                       .filter(([k]) => text.includes(k))
                       .map(([_,v]) => v);

  const dimMatch = text.match(/(\d{2,3})\s*[x×]\s*(\d{2,3})/);
  const width = dimMatch ? parseInt(dimMatch[1]) : 100;
  const height = dimMatch ? parseInt(dimMatch[2]) : 190;

  return { model_code, width, height, finish, glass, extras };
}

function generateCabinCode(request) {
  const extras_code = request.extras.map(e => e[0]).join('');
  return `${request.model_code}${request.finish}${request.glass}-${request.width}-${request.height}${extras_code}`;
}

function generateCabinUrl(request, baseUrl="https://bronzeapp.eu/AssembleCabinLink") {
  const extras_query = request.extras.map(e => `extra=${e}`).join('&');
  return `${baseUrl}?model=${request.model_code}&width=${request.width}&height=${request.height}&finish=${request.finish}&glass=${request.glass}&${extras_query}`;
}

export default async function handler(req, res) {
  const { text } = req.body;

  if (!text) {
    return res.status(400).json({ error: "Λείπει το πεδίο 'text' από το request body." });
  }

  const request = interpretRequest(text);
  const cabin_code = generateCabinCode(request);
  const cabin_url = generateCabinUrl(request);

  return res.status(200).json({
    model_code: request.model_code,
    width: request.width,
    height: request.height,
    finish: request.finish,
    glass: request.glass,
    extras: request.extras,
    cabin_code,
    cabin_url
  });
}

