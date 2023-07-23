import { CountriesCreationRequest, CountryCreationRequest } from "../generated/country.shared_pb";
import { CountryCreationModel } from "../models/countryCreationModel";

export class CountryCreationModelMapper {
    public static Map(countryCreationRequest: CountryCreationRequest, countryCreationModel: CountryCreationModel) {
        if(!countryCreationModel)
            return;

        countryCreationRequest.setName(countryCreationModel.name);
        countryCreationRequest.setDescription(countryCreationModel.description);
        countryCreationRequest.setAnthem(countryCreationModel.anthem);
        countryCreationRequest.setCapitalcity(countryCreationModel.capitalCity);
        countryCreationRequest.setFlaguri(countryCreationModel.flagUri);
        countryCreationRequest.setLanguagesList(countryCreationModel.languages);
    }

    public static Maps(countriesCreationRequest: CountriesCreationRequest, countriesCreationModel: CountryCreationModel[]) {
        if(!countriesCreationModel)
            return;

        countriesCreationModel.map(x => {
            let countryCreationRequest = new CountryCreationRequest();
            CountryCreationModelMapper.Map(countryCreationRequest, x);
            countriesCreationRequest.addCountries(countryCreationRequest);
        });
    }
}