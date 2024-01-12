import { Duration } from 'luxon';

export function useDistanceTimeFormat(){

    function formatDuration(seconds: number): string {
        if(seconds < 60){
          return Duration.fromObject({second: seconds}).toFormat("s' secondes'");
        }else if(seconds < 3600){
          return Duration.fromObject({second: seconds}).toFormat("m' min'");
        }else if (seconds < 86400) {
          return Duration.fromObject({second: seconds}).toFormat("hh' h 'm' min'");
        } else {
          return Duration.fromObject({second: seconds}).toFormat("j' jours et 'h' h'");
        }
      }
    
      function formatDistance(meters: number): string {
        if(meters < 1000)
          return `${meters} mètres`;
        else{
          const kilometers = Math.floor(meters/1000)
          return `${kilometers} kilomètres`;
        }
      }

     return {formatDistance, formatDuration} 
}