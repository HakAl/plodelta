// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import './App.css';
import InstructionsWithUpload from "./InstructionsWithUpload";
import {useState} from "react";
import GTOSelect from "./GTOSelect";
import {
    GTO_MIDSTAKES_PREFLOP_VALUES,
    GTO_POSTFLOP_AS_TITLES,
    GTO_POSTFLOP_AS_VALUES, GTO_POSTFLOP_VS_TITLES, GTO_POSTFLOP_VS_VALUES,
    GTO_PREFLOP_KEYS,
    REPORT_DEFAULT, SNOWFLAKE_INDEX
} from "./appData";
import GTOTable from "./GTOTable";

function App({reportSelect = false}) {
    const [report, setReport] = useState(REPORT_DEFAULT);
    const [preflopValues, setPreflopValues] = useState(null);
    const [postflopAsValues, setPostflopAsValues] = useState(null);
    const [postflopVsValues, setPostflopVsValues] = useState(null);
    const [preflopDeltas, setPreflopDeltas] = useState(null);
    const [postflopAsDeltas, setPostflopAsDeltas] = useState(null);
    const [postflopVsDeltas, setPostflopVsDeltas] = useState(null);
    const [preflopAverage, setpreflopAverage] = useState(null);
    const [postflopAsAverage, setPostflopAsAverage] = useState(null);
    const [postflopVsAverage, setPostflopVsAverage] = useState(null);

    //Callback to pass to Papaparse
    const complete = (results, file) => {
        const data = results.data;

        if (data && data.length > 1) {
            //clean values
            let values = data[1];
            //yuck
            let count = 0;
            values = values.map(value => {
                let result;
                // Handle missing/invalid values
                if (value === null || value === undefined || value === '' || isNaN(value)) {
                    result = null;
                } else if (count !== SNOWFLAKE_INDEX) {
                    result = (value * 100).toFixed(2);
                } else {
                    result = value;
                }
                count++;
                return result;
            })
            const preflopValues = values.slice(0, GTO_PREFLOP_KEYS.length);
            const postflopAsValues = values.slice(GTO_PREFLOP_KEYS.length, GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length);
            const postflopVsValues = values.slice((GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length));

            setPreflopValues(preflopValues);
            setPostflopAsValues(postflopAsValues);
            setPostflopVsValues(postflopVsValues);

            let preflopDeltas = [];
            let preflopSum = 0;
            let preflopValidCount = 0;
            for (let i = 0; i < GTO_MIDSTAKES_PREFLOP_VALUES.length; i++) {
                if (preflopValues[i] === null) {
                    preflopDeltas.push(null);
                } else {
                    let result = Math.abs((preflopValues[i] - GTO_MIDSTAKES_PREFLOP_VALUES[i]));
                    preflopDeltas.push(result.toFixed(2));
                    preflopSum += result;
                    preflopValidCount++;
                }
            }
            setpreflopAverage(preflopValidCount > 0 ? (preflopSum / preflopValidCount).toFixed(2) : null);
            setPreflopDeltas(preflopDeltas);

            let postflopAsDeltas = [];
            let postflopAsSum = 0;
            let postflopAsValidCount = 0;
            for (let i = 0; i < GTO_POSTFLOP_AS_VALUES.length; i++) {
                if (postflopAsValues[i] === null) {
                    postflopAsDeltas.push(null);
                } else {
                    let result = Math.abs((postflopAsValues[i] - GTO_POSTFLOP_AS_VALUES[i]));
                    postflopAsSum += result;
                    postflopAsDeltas.push(result.toFixed(2));
                    postflopAsValidCount++;
                }
            }
            setPostflopAsAverage(postflopAsValidCount > 0 ? (postflopAsSum / postflopAsValidCount).toFixed(2) : null);
            setPostflopAsDeltas(postflopAsDeltas);

            let postflopVsDeltas = [];
            let postflopVsSum = 0;
            let postflopVsValidCount = 0;
            for (let i = 0; i < GTO_POSTFLOP_VS_VALUES.length; i++) {
                if (postflopVsValues[i] === null) {
                    postflopVsDeltas.push(null);
                } else {
                    let result = Math.abs(postflopVsValues[i] - GTO_POSTFLOP_VS_VALUES[i]);
                    postflopVsSum += result;
                    postflopVsDeltas.push(result.toFixed(2));
                    postflopVsValidCount++;
                }
            }
            setPostflopVsAverage(postflopVsValidCount > 0 ? (postflopVsSum / postflopVsValidCount).toFixed(2) : null);
            setPostflopVsDeltas(postflopVsDeltas);
        }
    }

    const onReportChange = (event) => {
        if (event && event.target && event.target.value) {
            setReport(event.target.value);
        }
    }


    const preflopTableProps = {
        playerValues: preflopValues,
        playerDeltas: preflopDeltas,
        average: preflopAverage,
        title: "Preflop",
        gtoTitles: GTO_PREFLOP_KEYS,
        gtoValues: GTO_MIDSTAKES_PREFLOP_VALUES,
    }
    const postflopAsTableProps = {
        playerValues: postflopAsValues,
        playerDeltas: postflopAsDeltas,
        average: postflopAsAverage,
        title: "Postflop as Aggressor",
        gtoTitles: GTO_POSTFLOP_AS_TITLES,
        gtoValues: GTO_POSTFLOP_AS_VALUES,
    }
    const postflopVsTableProps = {
        playerValues: postflopVsValues,
        playerDeltas: postflopVsDeltas,
        average: postflopVsAverage,
        title: "Postflop VS Aggressor",
        gtoTitles: GTO_POSTFLOP_VS_TITLES,
        gtoValues: GTO_POSTFLOP_VS_VALUES,
    }

    return (
        <div className="App">
            <header className="App-header">
                <h1>GTO Coach</h1>
            </header>

            <section>
                <div className="container-fluid">
                    <div className="body-context">
                        <InstructionsWithUpload complete={complete}/>
                    </div>
                </div>
            </section>

            <section>
                <div className="container-fluid">
                    {reportSelect && <GTOSelect onReportChange={onReportChange} />}

                    {/*<HM3Report complete={complete}/>*/}

                    {preflopValues && (
                        <GTOTable
                            preflopTableProps={preflopTableProps}
                            postflopAsTableProps={postflopAsTableProps}
                            postflopVsTableProps={postflopVsTableProps}
                        />
                    )}
                </div>
            </section>

            <footer className="app-footer">
                <p>
                    Inspired by <a href="https://plomastermind.com" className="link" target="_blank" rel="noreferrer">PLO Mastermind</a>
                    {' '}&mdash;{' '}
                    <a href="https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/" className="link" target="_blank" rel="noreferrer">See this course</a>
                    {' '}and{' '}
                    <a href="https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119" className="link" target="_blank" rel="noreferrer">this document</a>
                </p>
            </footer>
        </div>
    );
}

export default App;