// v6: correct VS model code and extras pattern
export function generateCabinDetailCode({model_key,glass_key,glass_finish,width,height,extras}){
const MODEL_CODES={"vs":"8"};
const GLASS_CODES={"6mm":"2","6/8mm":"3","8mm":"4"};
const GLASS_FINISH_CODES={"fume":"4"};
const EXTRAS_CODES={"safekid":"2exp"};
const modelCode=MODEL_CODES[model_key];
const thicknessCode=GLASS_CODES[glass_key]||"2";
const glassFinishCode=GLASS_FINISH_CODES[glass_finish]||"0";
const widthMM=(width||100)*10;
const heightMM=(height||190)*10;
const extrasCode=extras.length?EXTRAS_CODES[extras[0]]||"nullexp":"nullexp";
return `${modelCode}-0-${thicknessCode}-${glassFinishCode}-1-0-0-${widthMM}-${heightMM}-${extrasCode}nullexpnullexxcccnullcccnullcccprtprt`;
}
