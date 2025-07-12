import { interpret_request, generate_cabin_code, generate_cabin_url } from '../../src/cabin_utils.js';

export default function handler(req, res) {
  const request = interpret_request(req.body.text || '');
  const cabin_code = generate_cabin_code(request);
  const cabin_url = generate_cabin_url(request);

  res.status(200).json({
    cabin_code,
    cabin_url
  });
}
