import './App.css';
import CSVInput from "./CSVInput";
import Instructions from "./Instructions";
import {useState} from "react";
import GTOSelect from "./GTOSelect";

function App() {
    const [report, setReport] = useState(null);

    return (
        <div className="App">
            <header className="App-header">
                <h1>Statistics Analyzer</h1>
            </header>
            <section>
                <div className={'body-context'}>
                    <h2>Compare your statistics to a standard set with <a href={'https://www.holdemmanager.com/hm3/download.php'} rel="noreferrer"className={'link'} target={'_blank'}>Holdem Manager 3</a></h2>
                   <Instructions/>
                </div>
            </section>
            <section>
                <GTOSelect />
                <CSVInput/>
            </section>

            <p className={'body-context'}>
                Inspired by  <a href={'https://plomastermind.com'}  className={'link'} target={'_blank'} rel="noreferrer">PLO Mastermind.</a>
                <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'} className={'link'} target={'_blank'} rel="noreferrer">See this course</a> and <a href={'https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119'} className={'link'} target={'_blank'} rel="noreferrer">this document</a>
            </p>
        </div>
    );
}

export default App;
