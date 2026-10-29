"""Extract small NES reference sprites for this educational reconstruction.
Artwork: Nintendo. Reference map: Rick N. Bruns / nesmaps.com.
"""
from pathlib import Path
from PIL import Image

root = Path(__file__).resolve().parents[1]
im = Image.open(root / 'Temp/References/2-2.png').convert('RGBA')
dest = root / 'Assets/Sprites'
blue = (32, 56, 236)
def save(name, box):
    sprite = im.crop(box)
    sprite.putdata([(r,g,b,0 if (r,g,b) == blue else 255) for r,g,b,a in sprite.getdata()])
    sprite.save(dest / (name + '.png'))
save('WaterGround', (0,448,16,464))
save('Coral', (176,400,192,416))
save('Blooper', (352,416,368,440))
# Find isolated red/gray fish components from their exact foreground bounds.
pixels = im.load()
seen = set()
found = False
for y in range(288,440):
    for x in range(0,3020):
        if (x,y) in seen or pixels[x,y][:3] == blue:
            continue
        stack=[(x,y)]; seen.add((x,y)); points=[]
        while stack:
            p,q=stack.pop(); points.append((p,q))
            for a,b in ((p-1,q),(p+1,q),(p,q-1),(p,q+1)):
                if 0 <= a < 3072 and 272 <= b < 448 and (a,b) not in seen and pixels[a,b][:3] != blue:
                    seen.add((a,b)); stack.append((a,b))
        xs,ys=zip(*points); w=max(xs)-min(xs)+1; h=max(ys)-min(ys)+1
        colors={pixels[p,q][:3] for p,q in points}
        if not found and 10<=w<=20 and 10<=h<=20 and (216,40,0) in colors:
            save('CheepCheep',(min(xs),min(ys),max(xs)+1,max(ys)+1)); found=True
            print('Fish',min(xs),min(ys),w,h)
