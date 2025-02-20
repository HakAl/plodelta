import {Fragment, useState} from "react";
import Papa from "papaparse";
import {
    GTO_MIDSTAKES_PREFLOP_VALUES,
    GTO_POSTFLOP_AS_TITLES,
    GTO_POSTFLOP_AS_VALUES,
    GTO_POSTFLOP_VS_TITLES,
    GTO_POSTFLOP_VS_VALUES,
    GTO_PREFLOP_KEYS
} from "./appData";
import GTOTable from "./GTOTable";

function CSVInput() {
    const [preflopValues, setPreflopValues] = useState(null);
    const [postflopAsValues, setPostflopAsValues] = useState(null);
    const [postflopVsValues, setPostflopVsValues] = useState(null);

    //Callback to pass to Papaparse
    const complete = (results, file) => {
        const data = results.data;

        if (data && data.length > 1) {
            //clean values
            let values = data[1];
            values = values.map(value => {
                return (value * 100).toFixed(1)
            })
            const preflopValues = values.slice(0, GTO_PREFLOP_KEYS.length);
            const postflopAsValues = values.slice(GTO_PREFLOP_KEYS.length, GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length);
            const postflopVsValues = values.slice((GTO_PREFLOP_KEYS.length +GTO_POSTFLOP_AS_TITLES.length));

            setPreflopValues(preflopValues);
            setPostflopAsValues(postflopAsValues);
            setPostflopVsValues(postflopVsValues);
        }
    }
    const onPreflopInputChange = (evt) => {
        if (evt && evt.target.files && evt.target.files.length) {
            Papa.parse(evt.target.files[0], {complete});
        }
    }
    const reportInputProps = {
        name: 'reportInput',
        type: "file",
        onChange: onPreflopInputChange,
    };
    const preflopTableProps = {
        playerValues: preflopValues,
        title: "Preflop",
        gtoTitles: GTO_PREFLOP_KEYS,
        gtoValues: GTO_MIDSTAKES_PREFLOP_VALUES
    }
    const postflopAsTableProps = {
        playerValues: postflopAsValues,
        title: "Postflop as Aggressor",
        gtoTitles: GTO_POSTFLOP_AS_TITLES,
        gtoValues: GTO_POSTFLOP_AS_VALUES
    }
    const postflopVsTableProps = {
        playerValues: postflopVsValues,
        title: "Postflop VS Aggressor",
        gtoTitles: GTO_POSTFLOP_VS_TITLES,
        gtoValues: GTO_POSTFLOP_VS_VALUES
    }

    return (
        <Fragment>
            <div className={'row'}>
                <div className={'input_container'}>
                    <input {...reportInputProps}/>
                </div>
            <div className={'column'}>
            <GTOTable {...preflopTableProps} />
                </div>
                <div className={'column'}>
                    <GTOTable {...postflopAsTableProps} />
                </div>
                <div className={'column'}>
                    <GTOTable {...postflopVsTableProps} />
                </div>
            </div>
        </Fragment>
    );
}

export default CSVInput;
