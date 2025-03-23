xs = []

for x in range(1, 10):
    xs.append("Tome0" + str(x))

for x in range(10, 23):
    xs.append("Tome" + str(x))

print(xs[::-1])