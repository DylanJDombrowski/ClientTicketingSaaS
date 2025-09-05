import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/dashboard',
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('../views/LoginView.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/register',
      name: 'Register',
      component: () => import('../views/RegisterView.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/dashboard',
      name: 'Dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/clients',
      name: 'Clients',
      component: () => import('../views/ClientsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/clients/:id',
      name: 'ClientDetails',
      component: () => import('../views/ClientDetailsView.vue'),
      meta: { requiresAuth: true },
      props: (route) => ({ id: Number(route.params.id) }),
    },
    {
      path: '/tickets',
      name: 'Tickets',
      component: () => import('../views/TicketsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/tickets/:id',
      name: 'TicketDetails',
      component: () => import('../views/TicketDetailsView.vue'),
      meta: { requiresAuth: true },
      props: (route) => ({ id: Number(route.params.id) }),
    },
    {
      path: '/profile',
      name: 'Profile',
      component: () => import('../views/ProfileView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/reports',
      name: 'Reports',
      component: () => import('../views/ReportsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('../views/NotFoundView.vue'),
    },
  ],
});

// Navigation guard for authentication
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();

  if (!authStore.currentUser && authStore.token) {
    await authStore.initialize();
  }

  const requiresAuth = to.meta.requiresAuth !== false;
  const isAuthenticated = authStore.isAuthenticated;

  if (requiresAuth && !isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } });
  } else if (
    !requiresAuth &&
    isAuthenticated &&
    (to.name === 'Login' || to.name === 'Register')
  ) {
    next({ name: 'Dashboard' });
  } else {
    next();
  }
});

export default router;
