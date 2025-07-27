.import re
from dataclasses import dataclass
from typing import List

@dataclass
class CabinRequest:
    model_code: str
    width: int
    height: int
    finish: str
    glass: str
    extras: List[str]

# Mappings
MODEL_MAP = {
    "γωνιακή": "9A",
    "συρόμενη": "9S",
    "διπλή συρόμενη": "94",
    "σταθερή": "9F",
    "ανοιγόμενη": "9B",
    "ημικυκλική": "9C",
    "inox": "VS",
}

FINISH_MAP = {
    "μαύρο ματ": "B",
    "λευκό ματ": "W",
    "ματ": "M",
    "γυαλιστερό": "G",
    "αντικέ": "A",
}

GLASS_MAP = {
    "σατινέ": "S",
    "διάφανο": "T",
    "φιμέ": "F",
    "σεριγραφία": "R",
}

EXTRAS_MAP = {
    "safekid": "SafeKid",
    "bronzeclean": "BronzeClean",
    "nano": "BronzeNano",
}

def interpret_request(text: str) -> CabinRequest:
    text = text.lower()
    extras = []
    
    model_code = next((v for k,v in MODEL_MAP.items() if k in text), "9S")
    finish = next((v for k,v in FINISH_MAP.items() if k in text), "G")
    glass = next((v for k,v in GLASS_MAP.items() if k in text), "T")

    extras = [v for k,v in EXTRAS_MAP.items() if k in text]

    dim_match = re.search(r"(\d{2,3})\s*[x×]\s*(\d{2,3})", text)
    width, height = (int(dim_match.group(1)), int(dim_match.group(2))) if dim_match else (100, 190)

    return CabinRequest(model_code, width, height, finish, glass, extras)

def generate_cabin_code(request: CabinRequest) -> str:
    extras_code = ''.join(e[0] for e in request.extras)
    return f"{request.model_code}{request.finish}{request.glass}-{request.width}-{request.height}{extras_code}"

def generate_cabin_url(request: CabinRequest, base_url="https://bronze.gr/AssembleCabinLink") -> str:
    extras_query = '&'.join(f"extra={e}" for e in request.extras)
    return f"{base_url}?model={request.model_code}&width={request.width}&height={request.height}&finish={request.finish}&glass={request.glass}&{extras_query}"

# Example usage
if __name__ == "__main__":
    user_input = "Θέλω καμπίνα γωνιακή, 120x190, μαύρο ματ, σατινέ, με BronzeClean και SafeKid"
    request = interpret_request(user_input)
    cabin_code = generate_cabin_code(request)
    cabin_url = generate_cabin_url(request)

    print("Cabin Code:", cabin_code)
    print("Cabin URL:", cabin_url)
