import './App.css';
import CSVInput from "./CSVInput";
import TheReport from "./Home_SavedReports_StatsAnalyzer.report";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <p>Statistics Analyzer</p>
            </header>
            <section>
                <div className={'body-context'}>
                    <p>This app is an aid to ease the pain points of comparing your Holdem Manager 3 statistics to GTO.</p>
                    <p>To analyze your statistics, you need to manually enter everything into HM3 to create a report. This is difficult and HM3 doesn't provide an easy way to share reports. It is possible to manually add reports to your HM3 app.</p>
                    <ol>
                        <li>
                            <a href={TheReport}
                               target="_blank"
                               rel="noreferrer"
                               download="Home_SavedReports_StatsAnalyzer.report">Download this file.</a>
                        </li>
                        <li>Find your Holdem Manager install directory. EG: "C:\Users\USERNAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\"</li>
                        <li>Paste the downloaded file into the saved reports directory: EG: C:\Users\USERNAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\Reports\Saved\</li>
                    </ol>
                </div>
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
