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
npm run build   # output → build/
```

## Deploy to GitHub Pages
```bash
npm run deploy  # pushes build/ to gh-pages branch
```

## Test
```bash
npm test        # Jest in watch mode
npm run test:coverage  # full coverage report
```

## Roadmap
### TODOS
- How do PF ranges differ based on stake / rake structure?
- Additional commentary on analysis
- summaries
- user stat input file upload
- csv output download
- investigate DH integration
- investigate data viz lib like https://www.npmjs.com/package/d3
- general HM3 report gen? HM3 interface sucks


## License
  MIT (see LICENSE file)