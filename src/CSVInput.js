import {Fragment, useState} from "react";
import Papa from "papaparse";
import {GTO_PREFLOP_KEYS} from "./appData";
import GTOTable from "./GTOTable";

function CSVInput() {
    const [playerValues, setPlayerValues] = useState(null);

    //Callback to pass to Papaparse
    const complete = (results, file) => {
        let data = results.data;

        if (data && data.length > 1) {
            //hm3 column titles
            let titles = data[0];

            //only take from allowlist
            let valuesToDelete = [];
            GTO_PREFLOP_KEYS.map((validTitle, i) => {
                titles = titles.filter(title => title !== validTitle);
            })

            titles = titles.filter(((item, i) => {
                const result = !titles.includes(item);
                if (!result) {
                    valuesToDelete.push(i);
                }
                return result;
            }));

            //clean values
            let values = data[1];
            valuesToDelete.sort();
            valuesToDelete.reverse();
            valuesToDelete.forEach((toKill) => {
                values.splice(toKill, 1);
            });
            //convert to percent, rounded 1 decimal
            values = values.map(value => {
                return (value * 100).toFixed(1)
            })
            setPlayerValues(values);
        }
    }

    const onCSVInputChange = (evt) => {
        if (evt && evt.target.files && evt.target.files.length) {
            Papa.parse(evt.target.files[0], {complete});
        }
    }

    const inputProps = {
        type: "file",
        onChange: onCSVInputChange,
        className: "csvButton"
    };
    const preflopTableProps = {
        playerValues,
        title: "Preflop Statistics",
    }

    return (
        <Fragment>
            <input {...inputProps} />
            <GTOTable {...preflopTableProps} />
            {/*{playerValues && <GTOTable {...preflopTableProps} />}*/}
        </Fragment>
    );
}

export default CSVInput;
