"""Generate original, deterministic chiptune audio. No downloaded music required."""
from pathlib import Path
import math
import struct
import wave

ROOT = Path(__file__).resolve().parents[1] / "Assets/Resources/Audio"
RATE = 22050

def write(name, notes, beat=.14):
    samples = []
    for pitch, beats in notes:
        duration = beat * beats
        count = int(duration * RATE)
        frequency = 440 * 2 ** ((pitch - 69) / 12) if pitch else 0
        for i in range(count):
            t = i / RATE
            envelope = min(1, t / .008) * min(1, (duration - t) / .025)
            value = (math.sin(2 * math.pi * frequency * t) + .25 * math.sin(6 * math.pi * frequency * t)) if pitch else 0
            samples.append(int(value * envelope * 11000))
    with wave.open(str(ROOT / (name + '.wav')), 'wb') as out:
        out.setparams((1, 2, RATE, 0, 'NONE', 'not compressed'))
        out.writeframes(struct.pack('<' + 'h' * len(samples), *samples))

def main():
    ROOT.mkdir(parents=True, exist_ok=True)
    effects = {
        'jump': [60, 67, 74, 81], 'stomp': [45, 38, 30],
        'coin': [83, 90], 'powerup': [60, 64, 67, 72, 76, 79],
        'damage': [70, 64, 58, 52], 'death': [76, 0, 75, 71, 67, 60, 48],
        'clear': [60, 64, 67, 72, 76, 79, 84, 0, 79, 84],
        'gameover': [67, 64, 60, 55, 59, 0, 48], 'swim': [60, 65, 69],
        'fire': [48, 43, 38, 33], 'switch': [72, 79, 84],
    }
    for name, pitches in effects.items():
        write(name, [(p, 1) for p in pitches], .065 if name not in ('clear', 'death', 'gameover') else .18)
    themes = {
        'overworld': [72, 76, 79, 76, 74, 77, 81, 77, 76, 79, 84, 79, 74, 71, 67, 0],
        'underground': [36, 48, 0, 39, 51, 0, 41, 53, 0, 39, 51, 0, 34, 46, 36, 0],
        'water': [60, 64, 67, 72, 71, 67, 62, 65, 69, 74, 72, 69, 64, 67, 71, 76, 74, 71],
        'castle': [36, 43, 48, 49, 48, 43, 35, 42, 47, 48, 47, 42, 34, 41, 46, 47],
    }
    for name, pitches in themes.items():
        write(name, [(p, 1) for p in pitches] * 4, .24 if name == 'water' else .17)

if __name__ == '__main__':
    main()
