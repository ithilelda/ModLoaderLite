import os
import math
from PIL import Image


def pack_directory(path):
    def name_comparer(entry):
        return int(entry.name.strip('image .png'))
    files = [e for e in os.scandir(path) if e.name.endswith('png') and not e.name.startswith('output')]
    files.reverse()

    images = [Image.open(e.path) for e in files]
    x = max([i.size[0] for i in images])
    y = max([i.size[1] for i in images])
    column = 32
    row = math.ceil(len(images) / column)
    output = Image.new('RGBA', (x * column, y * row))
    count = 0
    for i in images:
        c = count % column
        r = count // column
        x_center = c * x + x // 2
        y_center = r * y + y // 2
        width = i.size[0]
        height = i.size[1]
        output.paste(i, (x_center - width // 2, y_center - height // 2))
        count += 1
    output_name = os.path.join(path, 'output.png')
    output.save(output_name)


path = r'E:\export\fuwen'
pack_directory(path)