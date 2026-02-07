// LiveSensorChart.jsx
import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";

export default function LiveSensorChart({ data }) {
  if (!data || data.length === 0) return <div>No data yet...</div>;

  return (
    <ResponsiveContainer width="100%" height={150}>
      <LineChart data={data}>
        <XAxis 
          dataKey="timestamp" 
          tick={false}
          interval="preserveEnd"
        />
        <YAxis 
          // Keep float values in Y-axis
          tickFormatter={(value) => value.toFixed(2)} 
        />
        <Tooltip 
          formatter={(value) => value.toFixed(2)} 
        />
        <Line 
          type="monotone" 
          dataKey="value" 
          stroke="#3b82f6"
          dot={false}
          strokeWidth={2}
          isAnimationActive={false}
        />
      </LineChart>
    </ResponsiveContainer>
  );
}
