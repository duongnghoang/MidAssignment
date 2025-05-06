export default function PageTitle({ icon, title, subtitle, extraContent }) {
  return (
    <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6">
      <div className="mb-4 md:mb-0">
        <h1 className="text-2xl font-bold flex items-center">
          {icon} {title}
        </h1>
        <p className="text-gray-500">{subtitle}</p>
      </div>
      <div className="flex flex-wrap gap-2">{extraContent}</div>
    </div>
  );
}
