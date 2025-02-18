import './App.css';
import CSVInput from "./CSVInput";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <p>Statistics Analyzer</p>
            </header>
            <section>
                <div className={'body-instructions'}>
                    <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'}
                       className={'link'}
                       target={'_blank'}>For Context See This Course</a>
                    and
                    <a href={'https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119'}
                       className={'link'}
                       target={'_blank'}>This Document</a>
                    <ol>
                        <li>Create reports with the stats below</li>
                        <li>Right click the stats, "Select All"</li>
                        <li>Right click the stats, "Save As"</li>
                        <li>Use the buttons below to compare</li>
                    </ol>
                </div>
            </section>
            <section>
                <CSVInput/>
            </section>
        </div>
    );
}

export default App;
