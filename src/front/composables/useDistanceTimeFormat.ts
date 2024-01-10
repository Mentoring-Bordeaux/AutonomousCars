
export function useDistanceTimeFormat(){

    function formatDuration(seconds: number): string {
        if(seconds < 60){
          return `${seconds}s`;
        }else if(seconds < 3600){
          const minutes = Math.floor(seconds / 60);
          return `${minutes} min`;
        }else if (seconds < 86400) {
            const hours = Math.floor(seconds / 3600);
            const remainingMinutes = Math.floor((seconds % 3600) / 60);
            return `${hours} h ${remainingMinutes > 0 ? `${remainingMinutes} min` : ''}`;
        } else {
            const days = Math.floor(seconds / 86400);
            const remainingHours = Math.floor((seconds % 86400) / 3600);
            return `${days} jours ${remainingHours > 0 ? ` et ${remainingHours} h` : ''}`;
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