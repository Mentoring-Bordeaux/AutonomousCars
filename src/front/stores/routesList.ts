import type { routeStoreItem
 } from "~/models/routeStoreItem";
export const useRoutesListStore = defineStore('routesListStore', {
    state: () => ({
        routes: [] as routeStoreItem[],
    }), 
    actions: {
        getEndPosition(id: string){
           const route = this.routes.find((route) => id === route.id);
           if(route){
                return route.coordinates[route.coordinates.length-1];
           }
           return null; 
        },
        addOneRoute(route: routeStoreItem){
            const tmpRoutes = [...this.routes];
            tmpRoutes.push(route);
            this.routes = [...tmpRoutes]
        }, 
        addMultipleRoutes(routes: routeStoreItem[]){
            const tmpRoutes = [...this.routes]
            tmpRoutes.push(...routes);
            this.routes = [...tmpRoutes];
        }, 
        removeRoute(id: string){
            const tmpRoutes = this.routes.filter(item => item.id !== id);
            this.routes = [...tmpRoutes];
        },
        removeAllRoute(){
            this.routes = [];
        },
        removeSuggestedRoutes(){
            const tmpRoutes = this.routes.filter((suggestedRoutes) => suggestedRoutes.status !== "suggested");
            this.routes = [...tmpRoutes];
        },
        removeUsedRoutes(){
            const tmpRoutes = this.routes.filter((suggestedRoutes) => suggestedRoutes.status !== "used");
            this.routes = [...tmpRoutes];
        },
        getRouteById(id: string){
            const route = this.routes.find((route) => id === route.id);
            if(route)
                return {...route} as routeStoreItem; 
            return null; 
        },
        getRouteCoordinates(id: string, position: [number, number]){
            const route = this.routes.find((route) => id === route.id);
            if(route){
                const posIndex = route.coordinates.findIndex(c => 
                    c[0] === position[0] && c[1] === position[1]
                );
                if(posIndex !== -1){
                    return route.coordinates.slice(0, posIndex + 1);
                }
                return null; 
            }
            return null;
        },
        changeAndGetSelectedRoute(routeId: string, carId: string){
            const route = {... this.routes.find((route) => routeId === route.id)};
            if(route){
                route.status = "used";
                route.id = carId;
                return route as routeStoreItem;
            }
            return undefined; 
        },
        toggleClick(id: string) {
            const unselectedRoute = this.routes.find(item => item.click === true);
            if(unselectedRoute)
                unselectedRoute.click = false; 
            
            const selectedRoute = this.routes.find(item => item.id === id);
            if (selectedRoute) {
                selectedRoute.click = !selectedRoute.click;
                console.log(`Route nº${id} is selected in the store`);
            }
        }
    }
});