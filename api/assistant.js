// api/assistant.js

// api/assistant.js (wrapper προς finalCabinCode)
import { interpretRequest, generateCabinDetailCode } from './finalCabinCode.js';

export default function handler(req, res) {
  try {
    const { text } = req.body || {};
    if (!text) {
      return res.status(400).json({ error: "Λείπει το πεδίο 'text'" });
    }

    const parsedRequest = interpretRequest(text);
    if (!parsedRequest.model_key) {
      return res.status(400).json({ error: 'Δεν αναγνωρίστηκε μοντέλο καμπίνας.' });
    }

    const cabinCode = generateCabinDetailCode(parsedRequest);
    const cabinUrl  = `https://www.bronzeapp.eu/AssembleCabinLink/${cabinCode}`;

    return res.status(200).json({ cabin_url: cabinUrl, details: parsedRequest });
  } catch (err) {
    console.error('API error:', err);
    return res.status(500).json({ error: err.message || 'Unknown error' });
  }
}
