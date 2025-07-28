// api/finalCabinCode_v4.js
// Fix: only one "nullexp" after extrasCode

export function generateCabinDetailCode({ model_key, glass_key, width, height, extras }) {
  const MODEL_CODES = { "9s": "1" };
  const GLASS_CODES = { "6mm": "2" };
  const EXTRAS_CODES = { "safekid": "2exp" };
  const modelCode = MODEL_CODES[model_key];
  const thicknessCode = GLASS_CODES[glass_key] || "2";
  const widthMM = (width || 100) * 10;
  const heightMM = (height || 190) * 10;
  const extrasCode = extras.length ? EXTRAS_CODES[extras[0]] || "nullexp" : "nullexp";
  return `${modelCode}-0-${thicknessCode}-1-1-0-0-${widthMM}-${heightMM}-${extrasCode}nullexxcccnullcccprtprt`;
}
