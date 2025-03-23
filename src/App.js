import './App.css';
import HM3Report from "./HM3Report";
import Instructions from "./Instructions";
import {useState} from "react";
import GTOSelect from "./GTOSelect";
import {REPORT_DEFAULT} from "./appData";

function App({reportSelect = false}) {
    const [report, setReport] = useState(REPORT_DEFAULT);

    const onReportChange = (event) => {
        if (event && event.target && event.target.value) {
            setReport(event.target.value);
        }
    }

    return (
        <div className="App">
            <header className="App-header">
                <h1>Statistics Analyzer</h1>
            </header>
            <section>
                <div className={'body-context'}>
                    <h2>Compare your statistics to a standard set with <a href={'https://www.holdemmanager.com/hm3/download.php'} rel="noreferrer"className={'link'} target={'_blank'}>Holdem Manager 3</a></h2>
                   <Instructions selectedReport={report}/>
                </div>
            </section>
            <section>
                {reportSelect && <GTOSelect onReportChange={onReportChange} />}
                <HM3Report />
            </section>

            <p className={'body-context'}>
                Inspired by  <a href={'https://plomastermind.com'}  className={'link'} target={'_blank'} rel="noreferrer">PLO Mastermind.</a>
                <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'} className={'link'} target={'_blank'} rel="noreferrer">See this course</a> and <a href={'https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119'} className={'link'} target={'_blank'} rel="noreferrer">this document</a>
            </p>
        </div>
    );
}

export default App;
