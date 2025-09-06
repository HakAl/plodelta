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
#react app
npm run build   # output → build/

#dotnet helper tool
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

## Deploy to GitHub Pages

WIP --- Uses Pages to:

    Host a download page (static HTML) with a link to the EXE.
    Build the EXE in Actions and attach it to a release (or copy it into the gh-pages branch).
    Let users click the link → download → run locally.

```bash
npm run deploy  # pushes build/ to gh-pages branch

```

## Test
```bash
# React
npm test        # Jest in watch mode
npm run test:coverage  # full coverage report

# dotnet
# xUnit (recommended for .NET) and Moq for mocking dependencies

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