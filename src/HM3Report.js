// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {Fragment} from "react";
import Papa from "papaparse";

function HM3Report({complete}) {
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

    return (
        <Fragment>
            <div className={'input_container'}>
                <input {...reportInputProps}/>
            </div>
        </Fragment>
    );
}

export default HM3Report;
