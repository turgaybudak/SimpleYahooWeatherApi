# SimpleYahooWeatherApi
Weather forecast for Yahoo Weather Apps api  Sing in   https://developer.yahoo.com/  and  create App


Your Project
// add using 
using CloudWeather;

//info cWeatherID search in http://woeid.rosselliot.co.nz/lookup/nevsehir
//get Cloud Weather 
 
 
 // This Code
 var Forecasts = new WeatherForecast().GetWeatherForecast("your-cAppID", "your-cConsumerKey", "your-cApcConsumerSecretpID", "cWeatherID");
