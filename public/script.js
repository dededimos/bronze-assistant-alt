async function getCabinInfo(userText) {
  const response = await fetch("https://bronze-assistant-alt.vercel.app/api/finalCabinCode", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ text: userText })
  });

  if (!response.ok) {
    throw new Error(`API error: ${response.statusText}`);
  }

  return response.json();
}

async function submitCabinRequest() {
  const userText = document.getElementById('userText').value;
  
  try {
    const cabinInfo = await getCabinInfo(userText);
    
    document.getElementById('result').innerHTML = `
      <p><strong>Κωδικός καμπίνας:</strong> ${cabinInfo.cabin_code}</p>
      <p><strong>Link καμπίνας:</strong> <a href="${cabinInfo.cabin_url}" target="_blank">Προβολή καμπίνας</a></p>
    `;
  } catch (error) {
    document.getElementById('result').innerHTML = `Σφάλμα: ${error.message}`;
  }
}
