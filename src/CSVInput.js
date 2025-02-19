import {Fragment, useState} from "react";
import Papa from "papaparse";
import {
    GTO_KEYS,
    GTO_MIDSTAKES_PREFLOP_VALUES, GTO_POSTFLOP_AS_TITLES,
    GTO_POSTFLOP_AS_VALUES, GTO_POSTFLOP_VS_TITLES,
    GTO_POSTFLOP_VS_VALUES,
    GTO_PREFLOP_KEYS
} from "./appData";
import GTOTable from "./GTOTable";

function CSVInput() {
    const [playerValues, setPlayerValues] = useState(null);

    //Callback to pass to Papaparse
    const complete = (results, file) => {
        const data = results.data;

        if (data && data.length > 1) {
            //hm3 column titles
            let titles = data[0];

            //only take from allowlist
            let valuesToDelete = [];
            GTO_KEYS.map((validTitle, i) => {
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

    const onPreflopInputChange = (evt) => {
        if (evt && evt.target.files && evt.target.files.length) {
            Papa.parse(evt.target.files[0], {complete});
        }
    }


    const reportInputProps = {
        type: "file",
        onChange: onPreflopInputChange,
    };
    const preflopTableProps = {
        playerValues,
        title: "Preflop",
        gtoTitles: GTO_PREFLOP_KEYS,
        gtoValues: GTO_MIDSTAKES_PREFLOP_VALUES
    }
    const postflopAsTableProps = {
        playerValues,
        title: "Postflop as Preflop Aggressor",
        gtoTitles: GTO_POSTFLOP_AS_TITLES,
        gtoValues: GTO_POSTFLOP_AS_VALUES
    }
    const postflopVsTableProps = {
        playerValues,
        title: "Postflop as Preflop Aggressor",
        gtoTitles: GTO_POSTFLOP_VS_TITLES,
        gtoValues: GTO_POSTFLOP_VS_VALUES
    }

    return (
        <Fragment>
            <div className={'row'}>
                <div className={'input_container'}>
                    <input {...reportInputProps} />
                </div>
                <div className={'column'}>
                    <div className={'App-instructions'}>
                        <h2>Preflop</h2>
                    </div>
                    <GTOTable {...preflopTableProps} />
                </div>
                <div className={'column'}>
                    <div className={'App-instructions'}>
                        <h2>Postflop as PF Aggressor</h2>
                    </div>
                    <GTOTable {...postflopAsTableProps} />
                </div>
                <div className={'column'}>
                    <div className={'App-instructions'}>
                        <h2>Postflop vs. PF Aggressor</h2>
                    </div>
                    <GTOTable {...postflopVsTableProps} />
                </div>
            </div>
        </Fragment>
    );
}

export default CSVInput;
