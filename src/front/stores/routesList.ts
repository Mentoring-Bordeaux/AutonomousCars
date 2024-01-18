type routeStoreItem = {
    id: string
    coordinates: Array<[number, number]>;
    click: boolean
}

export const useRoutesListStore = defineStore('routesListStore', {
    state: () => ({
        routes: [] as routeStoreItem[],
    }), 
    actions: {
        addOneRoute(route: routeStoreItem){
            this.routes.push(route);
            console.log(`Route nº${route.id} added in the store`);
        }, 
        addMultipleRoutes(routes: routeStoreItem[]){
            this.routes = routes;
            console.log(`Multiple routesadded in the store`);
        }, 
        removeRoute(id: string){
            this.routes = this.routes.filter(item => item.id !== id);
            console.log(`Route nº${id} deleted in the store`);
        },
        removeAllRoute(){
            this.routes = [];
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