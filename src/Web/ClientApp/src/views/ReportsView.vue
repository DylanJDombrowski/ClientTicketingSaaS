<template>
  <div class="space-y-6">
    <!-- Header -->
    <div
      class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4"
    >
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Analytics & Reports</h1>
        <p class="mt-2 text-sm text-gray-600">
          Track your business performance and insights
        </p>
      </div>

      <!-- Date Range Picker -->
      <div class="flex items-center space-x-3">
        <div class="flex items-center space-x-2">
          <CalendarIcon class="h-5 w-5 text-gray-400" />
          <select
            v-model="selectedPeriod"
            @change="loadReportData"
            class="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500"
          >
            <option value="7d">Last 7 days</option>
            <option value="30d">Last 30 days</option>
            <option value="90d">Last 3 months</option>
            <option value="1y">Last year</option>
          </select>
        </div>
        <button
          @click="exportReports"
          class="bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-50 flex items-center space-x-2"
        >
          <ArrowDownTrayIcon class="h-4 w-4" />
          <span>Export</span>
        </button>
      </div>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div
        v-for="kpi in kpis"
        :key="kpi.name"
        class="bg-white rounded-lg shadow p-6"
      >
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="p-3 rounded-lg" :class="kpi.iconBg">
              <component
                :is="kpi.icon"
                class="h-6 w-6"
                :class="kpi.iconColor"
              />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">{{ kpi.name }}</p>
            <div class="flex items-baseline">
              <p class="text-2xl font-semibold text-gray-900">
                {{ kpi.value }}
              </p>
              <p
                v-if="kpi.change"
                class="ml-2 flex items-baseline text-sm font-semibold"
                :class="{
                  'text-green-600': kpi.trend === 'up',
                  'text-red-600': kpi.trend === 'down',
                  'text-gray-500': kpi.trend === 'neutral',
                }"
              >
                <ArrowUpIcon v-if="kpi.trend === 'up'" class="h-3 w-3 mr-0.5" />
                <ArrowDownIcon
                  v-if="kpi.trend === 'down'"
                  class="h-3 w-3 mr-0.5"
                />
                {{ kpi.change }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Ticket Status Distribution -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-lg font-medium text-gray-900">
            Ticket Status Distribution
          </h2>
          <select
            v-model="ticketChartType"
            class="text-sm border border-gray-300 rounded-lg px-2 py-1"
          >
            <option value="doughnut">Doughnut</option>
            <option value="bar">Bar Chart</option>
          </select>
        </div>

        <!-- Doughnut Chart -->
        <div v-if="ticketChartType === 'doughnut'" class="relative h-64">
          <canvas ref="ticketStatusChart" class="w-full h-full"></canvas>
        </div>

        <!-- Bar Chart -->
        <div v-else class="relative h-64">
          <canvas ref="ticketStatusBarChart" class="w-full h-full"></canvas>
        </div>

        <!-- Legend -->
        <div class="flex justify-center space-x-6 mt-4">
          <div class="flex items-center">
            <div class="w-3 h-3 bg-red-400 rounded-full mr-2"></div>
            <span class="text-sm text-gray-600"
              >Open ({{ ticketStatusData.open }})</span
            >
          </div>
          <div class="flex items-center">
            <div class="w-3 h-3 bg-yellow-400 rounded-full mr-2"></div>
            <span class="text-sm text-gray-600"
              >In Progress ({{ ticketStatusData.inProgress }})</span
            >
          </div>
          <div class="flex items-center">
            <div class="w-3 h-3 bg-green-400 rounded-full mr-2"></div>
            <span class="text-sm text-gray-600"
              >Completed ({{ ticketStatusData.completed }})</span
            >
          </div>
        </div>
      </div>

      <!-- Ticket Volume Trend -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-lg font-medium text-gray-900 mb-6">
          Ticket Volume Trend
        </h2>
        <div class="relative h-64">
          <canvas ref="ticketTrendChart" class="w-full h-full"></canvas>
        </div>
      </div>

      <!-- Time Tracking Summary -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-lg font-medium text-gray-900 mb-6">
          Time Tracking Summary
        </h2>
        <div class="relative h-64">
          <canvas ref="timeTrackingChart" class="w-full h-full"></canvas>
        </div>

        <!-- Summary Stats -->
        <div class="grid grid-cols-3 gap-4 mt-6 pt-4 border-t border-gray-200">
          <div class="text-center">
            <p class="text-sm text-gray-500">Total Hours</p>
            <p class="text-lg font-semibold text-gray-900">
              {{ timeTrackingData.totalHours }}h
            </p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-500">Billable Hours</p>
            <p class="text-lg font-semibold text-green-600">
              {{ timeTrackingData.billableHours }}h
            </p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-500">Revenue</p>
            <p class="text-lg font-semibold text-blue-600">
              ${{ timeTrackingData.totalRevenue }}
            </p>
          </div>
        </div>
      </div>

      <!-- Client Performance -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-lg font-medium text-gray-900 mb-6">
          Top Clients by Activity
        </h2>
        <div class="space-y-4">
          <div
            v-for="client in topClients"
            :key="client.name"
            class="flex items-center justify-between"
          >
            <div class="flex items-center space-x-3">
              <div
                class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center"
              >
                <span class="text-sm font-medium text-blue-600">
                  {{
                    client.name
                      .split(" ")
                      .map((n) => n[0])
                      .join("")
                  }}
                </span>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900">
                  {{ client.name }}
                </p>
                <p class="text-xs text-gray-500">
                  {{ client.ticketCount }} tickets
                </p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm font-medium text-gray-900">
                {{ client.hoursLogged }}h
              </p>
              <p class="text-xs text-gray-500">${{ client.revenue }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detailed Tables -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Activity -->
      <div class="bg-white rounded-lg shadow">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-lg font-medium text-gray-900">Recent Activity</h2>
        </div>
        <div class="divide-y divide-gray-200 max-h-80 overflow-y-auto">
          <div
            v-for="activity in recentActivity"
            :key="activity.id"
            class="p-4"
          >
            <div class="flex items-start space-x-3">
              <div
                class="w-2 h-2 rounded-full mt-2 flex-shrink-0"
                :class="{
                  'bg-green-400': activity.type === 'completed',
                  'bg-blue-400': activity.type === 'created',
                  'bg-yellow-400': activity.type === 'updated',
                  'bg-purple-400': activity.type === 'comment',
                }"
              ></div>
              <div class="flex-1 min-w-0">
                <p class="text-sm text-gray-900">{{ activity.description }}</p>
                <p class="text-xs text-gray-500 mt-1">
                  {{ formatRelativeTime(activity.timestamp) }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Performance Metrics -->
      <div class="bg-white rounded-lg shadow">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-lg font-medium text-gray-900">Performance Metrics</h2>
        </div>
        <div class="p-6">
          <div class="space-y-6">
            <!-- Average Response Time -->
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-700"
                  >Average Response Time</span
                >
                <span class="text-sm text-gray-900">2.3 hours</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div
                  class="bg-green-500 h-2 rounded-full"
                  style="width: 85%"
                ></div>
              </div>
              <p class="text-xs text-gray-500 mt-1">Target: 4 hours</p>
            </div>

            <!-- Resolution Rate -->
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-700"
                  >First Contact Resolution</span
                >
                <span class="text-sm text-gray-900">72%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div
                  class="bg-blue-500 h-2 rounded-full"
                  style="width: 72%"
                ></div>
              </div>
              <p class="text-xs text-gray-500 mt-1">Industry average: 68%</p>
            </div>

            <!-- Client Satisfaction -->
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-700"
                  >Client Satisfaction</span
                >
                <span class="text-sm text-gray-900">4.8/5</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div
                  class="bg-yellow-500 h-2 rounded-full"
                  style="width: 96%"
                ></div>
              </div>
              <p class="text-xs text-gray-500 mt-1">Based on 45 reviews</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick, watch } from "vue";
import {
  CalendarIcon,
  ArrowDownTrayIcon,
  ArrowUpIcon,
  ArrowDownIcon,
  TicketIcon,
  ClockIcon,
  CurrencyDollarIcon,
  UserGroupIcon,
} from "@heroicons/vue/24/outline";

// Chart.js setup - we'll simulate it for now
let Chart: any = null;

const selectedPeriod = ref("30d");
const ticketChartType = ref("doughnut");
const loading = ref(false);

// Chart refs
const ticketStatusChart = ref<HTMLCanvasElement>();
const ticketStatusBarChart = ref<HTMLCanvasElement>();
const ticketTrendChart = ref<HTMLCanvasElement>();
const timeTrackingChart = ref<HTMLCanvasElement>();

// Chart instances
let ticketStatusChartInstance: any = null;
let ticketStatusBarChartInstance: any = null;
let ticketTrendChartInstance: any = null;
let timeTrackingChartInstance: any = null;

// Data
const kpis = ref([
  {
    name: "Total Tickets",
    value: "284",
    change: "+12%",
    trend: "up",
    icon: TicketIcon,
    iconBg: "bg-blue-100",
    iconColor: "text-blue-600",
  },
  {
    name: "Avg Response Time",
    value: "2.3h",
    change: "-8%",
    trend: "up",
    icon: ClockIcon,
    iconBg: "bg-green-100",
    iconColor: "text-green-600",
  },
  {
    name: "Revenue",
    value: "$18,420",
    change: "+23%",
    trend: "up",
    icon: CurrencyDollarIcon,
    iconBg: "bg-yellow-100",
    iconColor: "text-yellow-600",
  },
  {
    name: "Active Clients",
    value: "47",
    change: "+5%",
    trend: "up",
    icon: UserGroupIcon,
    iconBg: "bg-purple-100",
    iconColor: "text-purple-600",
  },
]);

const ticketStatusData = ref({
  open: 45,
  inProgress: 28,
  completed: 211,
});

const timeTrackingData = ref({
  totalHours: 642,
  billableHours: 518,
  totalRevenue: "38,850",
});

const topClients = ref([
  { name: "Acme Corp", ticketCount: 23, hoursLogged: 89, revenue: "6,675" },
  {
    name: "Tech Solutions",
    ticketCount: 18,
    hoursLogged: 67,
    revenue: "5,025",
  },
  { name: "StartUp Inc", ticketCount: 15, hoursLogged: 42, revenue: "3,150" },
  {
    name: "Digital Agency",
    ticketCount: 12,
    hoursLogged: 35,
    revenue: "2,625",
  },
  { name: "Local Business", ticketCount: 8, hoursLogged: 24, revenue: "1,800" },
]);

const recentActivity = ref([
  {
    id: 1,
    type: "completed",
    description: 'Ticket #156 "Email setup issue" was completed',
    timestamp: new Date(Date.now() - 1000 * 60 * 30), // 30 minutes ago
  },
  {
    id: 2,
    type: "created",
    description:
      'New ticket #157 "Website loading slowly" was created by Acme Corp',
    timestamp: new Date(Date.now() - 1000 * 60 * 60), // 1 hour ago
  },
  {
    id: 3,
    type: "comment",
    description: "John Smith added a comment to ticket #155",
    timestamp: new Date(Date.now() - 1000 * 60 * 90), // 1.5 hours ago
  },
  {
    id: 4,
    type: "updated",
    description: "Ticket #154 priority changed to High",
    timestamp: new Date(Date.now() - 1000 * 60 * 120), // 2 hours ago
  },
  {
    id: 5,
    type: "completed",
    description: 'Ticket #153 "Database backup" was completed',
    timestamp: new Date(Date.now() - 1000 * 60 * 180), // 3 hours ago
  },
]);

// Methods
const loadReportData = async () => {
  loading.value = true;
  try {
    // Mock API call - replace with real data fetching
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // Update KPIs based on selected period
    if (selectedPeriod.value === "7d") {
      kpis.value[0].value = "42";
      kpis.value[1].value = "1.8h";
      kpis.value[2].value = "$3,200";
      kpis.value[3].value = "24";
    } else if (selectedPeriod.value === "30d") {
      kpis.value[0].value = "284";
      kpis.value[1].value = "2.3h";
      kpis.value[2].value = "$18,420";
      kpis.value[3].value = "47";
    }

    // Update charts
    await nextTick();
    initializeCharts();
  } catch (error) {
    console.error("Failed to load report data:", error);
  } finally {
    loading.value = false;
  }
};

const exportReports = () => {
  // Mock export functionality
  const data = {
    period: selectedPeriod.value,
    kpis: kpis.value,
    ticketStatus: ticketStatusData.value,
    timeTracking: timeTrackingData.value,
    topClients: topClients.value,
  };

  const blob = new Blob([JSON.stringify(data, null, 2)], {
    type: "application/json",
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `analytics-report-${selectedPeriod.value}.json`;
  a.click();
  URL.revokeObjectURL(url);
};

const formatRelativeTime = (date: Date) => {
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);

  if (days > 0) return `${days} day${days > 1 ? "s" : ""} ago`;
  if (hours > 0) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
  if (minutes > 0) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
  return "Just now";
};

// Mock Chart.js functionality
const createMockChart = (canvas: HTMLCanvasElement, config: any) => {
  if (!canvas) return null;

  const ctx = canvas.getContext("2d");
  if (!ctx) return null;

  // Clear canvas
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  // Set canvas size
  const rect = canvas.getBoundingClientRect();
  canvas.width = rect.width * devicePixelRatio;
  canvas.height = rect.height * devicePixelRatio;
  ctx.scale(devicePixelRatio, devicePixelRatio);

  if (config.type === "doughnut") {
    drawDoughnutChart(ctx, config.data, rect.width, rect.height);
  } else if (config.type === "bar") {
    drawBarChart(ctx, config.data, rect.width, rect.height);
  } else if (config.type === "line") {
    drawLineChart(ctx, config.data, rect.width, rect.height);
  }

  return { destroy: () => ctx.clearRect(0, 0, canvas.width, canvas.height) };
};

const drawDoughnutChart = (
  ctx: CanvasRenderingContext2D,
  data: any,
  width: number,
  height: number
) => {
  const centerX = width / 2;
  const centerY = height / 2;
  const radius = Math.min(width, height) / 3;
  const innerRadius = radius * 0.6;

  const total = data.datasets[0].data.reduce(
    (sum: number, value: number) => sum + value,
    0
  );
  const colors = data.datasets[0].backgroundColor;

  let currentAngle = -Math.PI / 2;

  data.datasets[0].data.forEach((value: number, index: number) => {
    const sliceAngle = (value / total) * 2 * Math.PI;

    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle);
    ctx.arc(
      centerX,
      centerY,
      innerRadius,
      currentAngle + sliceAngle,
      currentAngle,
      true
    );
    ctx.fillStyle = colors[index];
    ctx.fill();

    currentAngle += sliceAngle;
  });
};

const drawBarChart = (
  ctx: CanvasRenderingContext2D,
  data: any,
  width: number,
  height: number
) => {
  const padding = 40;
  const chartWidth = width - padding * 2;
  const chartHeight = height - padding * 2;

  const maxValue = Math.max(...data.datasets[0].data);
  const barWidth = (chartWidth / data.labels.length) * 0.6;
  const barSpacing = (chartWidth / data.labels.length) * 0.4;

  // Draw bars
  data.datasets[0].data.forEach((value: number, index: number) => {
    const barHeight = (value / maxValue) * chartHeight;
    const x = padding + index * (barWidth + barSpacing) + barSpacing / 2;
    const y = height - padding - barHeight;

    ctx.fillStyle = data.datasets[0].backgroundColor[index];
    ctx.fillRect(x, y, barWidth, barHeight);

    // Draw value labels
    ctx.fillStyle = "#374151";
    ctx.font = "12px Arial";
    ctx.textAlign = "center";
    ctx.fillText(value.toString(), x + barWidth / 2, y - 5);
  });
};

const drawLineChart = (
  ctx: CanvasRenderingContext2D,
  data: any,
  width: number,
  height: number
) => {
  const padding = 40;
  const chartWidth = width - padding * 2;
  const chartHeight = height - padding * 2;

  const maxValue = Math.max(...data.datasets[0].data);
  const stepX = chartWidth / (data.labels.length - 1);

  // Draw line
  ctx.strokeStyle = data.datasets[0].borderColor;
  ctx.lineWidth = 3;
  ctx.beginPath();

  data.datasets[0].data.forEach((value: number, index: number) => {
    const x = padding + index * stepX;
    const y = height - padding - (value / maxValue) * chartHeight;

    if (index === 0) {
      ctx.moveTo(x, y);
    } else {
      ctx.lineTo(x, y);
    }

    // Draw points
    ctx.fillStyle = data.datasets[0].borderColor;
    ctx.beginPath();
    ctx.arc(x, y, 4, 0, 2 * Math.PI);
    ctx.fill();
  });

  ctx.stroke();
};

const initializeCharts = async () => {
  await nextTick();

  // Destroy existing charts
  if (ticketStatusChartInstance) ticketStatusChartInstance.destroy();
  if (ticketStatusBarChartInstance) ticketStatusBarChartInstance.destroy();
  if (ticketTrendChartInstance) ticketTrendChartInstance.destroy();
  if (timeTrackingChartInstance) timeTrackingChartInstance.destroy();

  // Ticket Status Doughnut Chart
  if (ticketStatusChart.value) {
    ticketStatusChartInstance = createMockChart(ticketStatusChart.value, {
      type: "doughnut",
      data: {
        labels: ["Open", "In Progress", "Completed"],
        datasets: [
          {
            data: [
              ticketStatusData.value.open,
              ticketStatusData.value.inProgress,
              ticketStatusData.value.completed,
            ],
            backgroundColor: ["#EF4444", "#F59E0B", "#10B981"],
          },
        ],
      },
    });
  }

  // Ticket Status Bar Chart
  if (ticketStatusBarChart.value) {
    ticketStatusBarChartInstance = createMockChart(ticketStatusBarChart.value, {
      type: "bar",
      data: {
        labels: ["Open", "In Progress", "Completed"],
        datasets: [
          {
            data: [
              ticketStatusData.value.open,
              ticketStatusData.value.inProgress,
              ticketStatusData.value.completed,
            ],
            backgroundColor: ["#EF4444", "#F59E0B", "#10B981"],
          },
        ],
      },
    });
  }

  // Ticket Trend Line Chart
  if (ticketTrendChart.value) {
    ticketTrendChartInstance = createMockChart(ticketTrendChart.value, {
      type: "line",
      data: {
        labels: ["Week 1", "Week 2", "Week 3", "Week 4"],
        datasets: [
          {
            data: [42, 38, 51, 47],
            borderColor: "#3B82F6",
            backgroundColor: "#3B82F6",
          },
        ],
      },
    });
  }

  // Time Tracking Chart
  if (timeTrackingChart.value) {
    timeTrackingChartInstance = createMockChart(timeTrackingChart.value, {
      type: "bar",
      data: {
        labels: ["Billable", "Non-billable"],
        datasets: [
          {
            data: [
              timeTrackingData.value.billableHours,
              timeTrackingData.value.totalHours -
                timeTrackingData.value.billableHours,
            ],
            backgroundColor: ["#10B981", "#6B7280"],
          },
        ],
      },
    });
  }
};

// Watch for chart type changes
watch(ticketChartType, () => {
  initializeCharts();
});

// Lifecycle
onMounted(() => {
  loadReportData();
});

onUnmounted(() => {
  // Cleanup charts
  if (ticketStatusChartInstance) ticketStatusChartInstance.destroy();
  if (ticketStatusBarChartInstance) ticketStatusBarChartInstance.destroy();
  if (ticketTrendChartInstance) ticketTrendChartInstance.destroy();
  if (timeTrackingChartInstance) timeTrackingChartInstance.destroy();
});
</script>

<style scoped>
canvas {
  max-width: 100%;
  height: auto;
}
</style>
