export default async function handler(req, res) {
  const { text } = req.body;

  // Εδώ μπορείς να ενσωματώσει η λογική του βοηθού
  res.status(200).json({ 
    message: "Το endpoint λειτουργεί!",
    received: text 
  });
}
