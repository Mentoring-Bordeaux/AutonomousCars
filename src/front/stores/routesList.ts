import type { routeStoreItem
 } from "~/models/routeStoreItem";
export const useRoutesListStore = defineStore('routesListStore', {
    state: () => ({
        routes: [] as routeStoreItem[],
    }), 
    actions: {
        deepCopy(){
            return this.routes.map((element) => ({...element})) as routeStoreItem[];
        },
        addOneRoute(route: routeStoreItem){
            const tmpRoutes = [...this.routes];
            tmpRoutes.push(route);
            this.routes = [...tmpRoutes]
            console.log(`Routes added in the store`);
        }, 
        addMultipleRoutes(routes: routeStoreItem[]){
            const tmpRoutes = [...this.routes]
            tmpRoutes.push(...routes);
            this.routes = [...tmpRoutes];
            console.log(`Multiple routes added in the store`);
        }, 
        removeRoute(id: string){
            const tmpRoutes = this.routes.filter(item => item.id !== id);
            this.routes = [...tmpRoutes];
            console.log(`Route nº${id} deleted in the store`);
        },
        removeAllRoute(){
            this.routes = [];
        },
        removeSuggestedRoutes(){
            const tmpRoutes = this.routes.filter((suggestedRoutes) => suggestedRoutes.status !== "suggested");
            this.routes = [...tmpRoutes];
        },
        getRouteById(id: string){
            const route = this.routes.find((route) => id === route.id);
            if(route)
                return {...route} as routeStoreItem; 
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