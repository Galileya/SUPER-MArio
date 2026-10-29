"""Trace the solid tile layout from Rick N. Bruns' annotated NES reference maps.
References are only inputs; generated text contains tile IDs, not map artwork.
Place source PNGs under Temp/References before running.
"""
from collections import Counter
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
DEST = ROOT / 'Assets/Levels'
DEST.mkdir(exist_ok=True)
for name in ('1-2', '2-2'):
    im = Image.open(ROOT / 'Temp/References' / (name + '.png')).convert('RGB')
    tiles = [im.crop((x, y, x + 16, y + 16)).tobytes()
             for y in range(272, 480, 16) for x in range(0, 3072, 16)]
    common = Counter(tiles).most_common()
    indices = {raw: i for i, (raw, count) in enumerate(common)}
    # B brick, G ground, H hard block, C coin, ? question, P pipe, R coral.
    mapping = ({1:'B', 2:'G', 3:'H', 4:'P', 5:'P', 6:'C', 9:'P', 10:'P', 15:'?', 24:'B', 31:'C', 33:'H'}
               if name == '1-2' else {1:'G', 3:'R', 4:'C', 32:'P', 37:'P'})
    rows = [''.join(mapping.get(indices[tiles[y * 192 + x]], '.') for x in range(192)) for y in range(13)]
    (DEST / (name + '.txt')).write_text('\n'.join(rows) + '\n', encoding='utf-8')
    print(name, Counter(''.join(rows)))
