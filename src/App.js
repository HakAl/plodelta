// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import './App.css';
import HM3Report from "./HM3Report";
import Instructions from "./Instructions";
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
                if (count !== SNOWFLAKE_INDEX) {
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
            for (let i = 0; i < GTO_MIDSTAKES_PREFLOP_VALUES.length; i++) {
                let result = Math.abs((preflopValues[i] - GTO_MIDSTAKES_PREFLOP_VALUES[i]));
                preflopDeltas.push(result.toFixed(2));
                preflopSum += result;
            }
            setpreflopAverage((preflopSum / GTO_MIDSTAKES_PREFLOP_VALUES.length).toFixed(2))
            setPreflopDeltas(preflopDeltas);

            let postflopAsDeltas = [];
            let postflopAsSum = 0;
            for (let i = 0; i < GTO_POSTFLOP_AS_VALUES.length; i++) {
                let result = Math.abs((postflopAsValues[i] - GTO_POSTFLOP_AS_VALUES[i]));
                postflopAsSum += result;
                postflopAsDeltas.push(result.toFixed(2));
            }
            setPostflopAsAverage((postflopAsSum / GTO_POSTFLOP_AS_VALUES.length).toFixed(2))
            setPostflopAsDeltas(postflopAsDeltas);

            let postflopVsDeltas = [];
            let postflopVsSum = 0;
            for (let i = 0; i < GTO_POSTFLOP_VS_VALUES.length; i++) {
                let result = Math.abs(postflopVsValues[i] - GTO_POSTFLOP_VS_VALUES[i]);
                postflopVsSum += result;
                postflopVsDeltas.push(result.toFixed(2));
            }
            setPostflopVsAverage((postflopVsSum / GTO_POSTFLOP_VS_VALUES.length).toFixed(2));
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
                <div className={'body-context'}>
                    <h2>Analyze your GTO report with <a href={'https://www.holdemmanager.com/hm3/download.php'} rel="noreferrer"className={'link'} target={'_blank'}>Holdem Manager 3</a></h2>
                   <Instructions selectedReport={report}/>
                </div>
            </section>
            <section>
                {reportSelect && <GTOSelect onReportChange={onReportChange} />}

                <div className={'row'}>
                    <HM3Report complete={complete}/>
                    { preflopValues && <GTOTable preflopTableProps={preflopTableProps} postflopAsTableProps={postflopAsTableProps} postflopVsTableProps={postflopVsTableProps} /> }
                </div>
            </section>

            <p className={'body-context'}>
                Inspired by  <a href={'https://plomastermind.com'}  className={'link'} target={'_blank'} rel="noreferrer">PLO Mastermind.</a>
                <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'} className={'link'} target={'_blank'} rel="noreferrer">See this course</a> and <a href={'https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119'} className={'link'} target={'_blank'} rel="noreferrer">this document</a>
            </p>
        </div>
    );
}

export default App;
