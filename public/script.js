async function getCabinInfo(userText) {
  const response = await fetch(;
if (!response.ok) {
  let data = {};
  try { data = await response.json(); } catch (_) {}
  throw new Error(`API error: ${data.error || response.statusText}`);
}
  return response.json();
}
async function submitCabinRequest() {
  const userText = document.getElementById('userText').value;

  try {
    const cabinInfo = await getCabinInfo(userText);
    const details = cabinInfo.details;

    document.getElementById('result').innerHTML = `
      <p><strong>Κωδικός καμπίνας (debug):</strong> ${cabinInfo.internal_debug_code}</p>
      <p><strong>Link καμπίνας:</strong> 
        <a href="${cabinInfo.cabin_url}" target="_blank">Προβολή καμπίνας</a>
      </p>
      <hr>
      <h4>Λεπτομέρειες:</h4>
      <p><strong>Μοντέλο:</strong> ${details.model_key?.toUpperCase() || '—'}</p>
      <p><strong>Διαστάσεις:</strong> ${details.width || '?'} x ${details.height || '?'} cm</p>
      <p><strong>Πάχος γυαλιού:</strong> ${details.glass_thickness || '—'}</p>
      <p><strong>Extras:</strong> ${details.extras.length ? details.extras.join(', ') : '—'}</p>
    `;
  } catch (error) {
    document.getElementById('result').innerHTML = `Σφάλμα: ${error.message}`;
  }
}
