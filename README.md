# PLO Delta

Simple proof of concept React app that ingests CSV files, crunches a few numbers (averages, etc.), and spits out a clean report.  
Built in a weekend because spreadsheets shouldn’t be the only option.
It shows where your stats are more than 10% off with colors!

## Demo
https://hakal.github.io/plodelta/

## Install & Run
```bash
git clone https://github.com/HakAl/plodelta.git
cd plodelta
npm ci          # faster, reproducible install
npm start       # dev server on http://localhost:3000
```

## Build for production
```bash
#dotnet helper tool
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

## Deploy to GitHub Pages

Triggered on ```git push```



## License
MIT (see LICENSE file)