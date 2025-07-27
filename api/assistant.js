function generateCabinCode(request) {
  const extras_code = request.extras.map(e => e[0]).join('');
  return `${request.model_code.toLowerCase()}${request.width}-${request.finish.toLowerCase()}-${request.height}${extras_code}`;
}

function generateCabinUrl(request, baseUrl = "https://www.bronzeapp.eu/AssembleCabinLink") {
  const cabinCode = generateCabinCode(request);
  return `${baseUrl}/${cabinCode}`;
}
