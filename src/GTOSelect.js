import {REPORT_VALUES} from "./appData";

function GTOSelect() {
    return <div className={'input_container'}>
        <label htmlFor="gto-select">Choose GTO Standard:</label>
        <select name="reports" id="reports_select">
            {REPORT_VALUES.map((report) => <option value={report.value}>{report.display}</option>)}
        </select>
    </div>
}

export default GTOSelect;