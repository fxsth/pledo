import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import {Settings} from "./components/Settings";
import {Movies} from "./components/Movies";
// import {Series} from "./components/Series";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },{
    path: '/settings',
    element: <Settings/>
  },{
    path: '/movies',
    element: <Movies/>
  }
];

export default AppRoutes;
