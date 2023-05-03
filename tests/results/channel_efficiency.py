import pandas as pd
import matplotlib as plt

df = pd.read_csv("fixed-muting-fbe.csv", delimiter="|")
df = df.groupby(["name", "simulation_run"])["air_time"].mean().reset_index()
df = df.groupby(["simulation_run"])["air_time"].sum()
df["air_time"] = df["air_time"].div(20_000_000)
ax = df.plot(marker="-", legend=False)
ax.set(xlabel="Station number", ylabel="Channel efficiency")
plt.tight_layout()
plt.savefig("test.png")
