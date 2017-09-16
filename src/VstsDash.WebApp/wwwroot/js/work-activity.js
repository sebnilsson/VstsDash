(function() {
    google.charts.load("current", { 'packages': ["corechart"], 'language': "en" });
    google.charts.setOnLoadCallback(onGoogleChartsLoad);

    var params = {};

    var charts = {
        draw: {
            all: function() {
                this.teamActivities();
                this.teamDoneEfforts();
                this.memberActivities();
            },
            teamActivities: function() {
                var $element = $("#team-activities-chart"),
                    element = $element[0];

                if (!element) {
                    return;
                }

                charts.data.getTeamActivityJson(function(data) {
                    var mappedData = charts.data.getMappedData(data);

                    var dataTable = charts.googleCharts.getActivitiesDataTable(mappedData);

                    var options = charts.googleCharts.getOptions(mappedData);

                    options.pointSize = 5;
                    options.chartArea = {
                        width: "85%",
                        height: "85%"
                    };

                    charts.googleCharts.drawLineChart(dataTable, options, element);
                });
            },
            teamDoneEfforts: function() {
                var $element = $("#team-done-efforts-chart"),
                    element = $element[0];

                if (!element) {
                    return;
                }

                charts.data.getTeamDoneEffortsJson(function(data) {
                    var mappedData = charts.data.getMappedData(data);

                    var dataTable = charts.googleCharts.getDoneEffortsDataTable(mappedData);

                    var options = charts.googleCharts.getOptions(mappedData);

                    options.legend.position = "none";
                    options.pointSize = 5;
                    options.chartArea = {
                        width: "85%",
                        height: "85%"
                    };
                    options.vAxes = {
                        0: { title: "Effort done" }
                    };
                    options.vAxes[0].minValue = 10;

                    options.trendlines = {
                        0: {
                            type: 'polynomial', // 'linear'
                            color: '#000000',
                            opacity: 0.5,
                            pointsVisible: false,
                            showR2: true
                        }
                    };

                    charts.googleCharts.drawLineChart(dataTable, options, element);
                });
            },
            memberActivities: function() {
                var $memberActivities = $(".activity-member-activity");

                for (var i = 0; i < $memberActivities.length; i++) {
                    memberActivity(i);
                }

                function memberActivity(index) {
                    var $memberActivity = $memberActivities.eq(index),
                        element = $memberActivity[0],
                        memberId = $memberActivity.data("memberId");

                    if (!element) {
                        return;
                    }

                    charts.data.getMemberActivityJson(
                        memberId,
                        function(data) {
                            var mappedData = charts.data.getMappedData(data);

                            var dataTable = charts.googleCharts.getActivitiesDataTable(mappedData);

                            var options = charts.googleCharts.getOptions(mappedData);

                            options.pointSize = 3;
                            options.legend.position = "none";
                            options.chartArea = {
                                width: "100%",
                                height: "100%"
                            };

                            if (params.memberMaxCommits) {
                                options.vAxes[0].minValue = 0;
                                options.vAxes[0].maxValue = params.memberMaxCommits;
                            }
                            if (params.memberMaxChanges) {
                                options.vAxes[1].minValue = 0;
                                options.vAxes[1].maxValue = params.memberMaxChanges;
                            }
                            
                            charts.googleCharts.drawLineChart(dataTable, options, element);
                        });
                }
            }
        },
        data: {
            getMappedData: function(data) {
                return data.map(function(x) {
                    x[0] = new Date(x[0]);
                    return x;
                });
            },
            getMemberActivityJson: function(memberId, callbackFn) {
                var memberActivityUrl = "/api/work/memberactivities/" +
                    ((memberId || "") + "?") +
                    (params.iterationId ? "iterationId=" + params.iterationId + "&" : "") +
                    (params.projectId ? "projectId=" + params.projectId + "&" : "") +
                    (params.teamId ? "teamId=" + params.teamId : "");

                $.getJSON(memberActivityUrl, callbackFn);
            },
            getTeamActivityJson: function(callbackFn) {
                var teamActivitiesUrl = "/api/work/teamactivities?" +
                    (params.iterationId ? "iterationId=" + params.iterationId + "&" : "") +
                    (params.projectId ? "projectId=" + params.projectId + "&" : "") +
                    (params.teamId ? "teamId=" + params.teamId : "");

                $.getJSON(teamActivitiesUrl, callbackFn);
            },
            getTeamDoneEffortsJson: function(callbackFn) {
                var teamDoneEffortsJsonUrl = "/api/work/teamdoneefforts?" +
                    (params.iterationId ? "iterationId=" + params.iterationId + "&" : "") +
                    (params.projectId ? "projectId=" + params.projectId + "&" : "") +
                    (params.teamId ? "teamId=" + params.teamId : "");

                $.getJSON(teamDoneEffortsJsonUrl, callbackFn);
            }
        },
        googleCharts: {
            getOptions: function(data) {
                var hAxisMinValue = params.fromDate ? new Date(params.fromDate) : data[0][0];
                var hAxisMaxValue = params.toDate ? new Date(params.toDate) : data[data.length - 1][0];

                var options = {
                    curveType: "function",
                    legend: { position: "top" },
                    series: {
                        0: { targetAxisIndex: 0 },
                        1: { targetAxisIndex: 1 }
                    },
                    hAxis: {
                        format: "yyyy-MM-dd",
                        minValue: hAxisMinValue,
                        maxValue: hAxisMaxValue
                    },
                    vAxes: {
                        0: { title: "Commits" },
                        1: { title: "Changes" }
                    }
                };
                return options;
            },
            getActivitiesDataTable: function(data) {
                var dataTable = new google.visualization.DataTable();
                dataTable.addColumn({ label: "Date", type: "date", role: "domain" });
                dataTable.addColumn({ label: "Commits", type: "number", role: "data" });
                dataTable.addColumn({ label: "Changes", type: "number", role: "data" });
                dataTable.addRows(data);

                return dataTable;
            },
            getDoneEffortsDataTable: function(data) {
                var dataTable = new google.visualization.DataTable();
                dataTable.addColumn({ label: "Date", type: "date", role: "domain" });
                dataTable.addColumn({ label: "Done Effort", type: "number", role: "data" });
                dataTable.addRows(data);

                return dataTable;

            },
            drawLineChart: function(dataTable, options, element) {
                var chart = new google.visualization.LineChart(element);

                chart.draw(dataTable, options);
            }
        }
    };

    function getParams() {
        var $activityContainer = $(".activity-container");
        if (!$activityContainer.length) {
            throw Error("Could not find activity-container-element.");
        }

        return {
            iterationId: $activityContainer.data("qsIterationId"),
            projectId: $activityContainer.data("qsProjectId"),
            teamId: $activityContainer.data("qsTeamId"),
            fromDate: parseFloat($activityContainer.data("fromDate")) || "",
            toDate: parseFloat($activityContainer.data("toDate")) || "",
            memberMaxCommits: parseInt($activityContainer.data("memberMaxCommits"), 10) || "",
            memberMaxChanges: parseInt($activityContainer.data("memberMaxChanges"), 10) || ""
        };
    }

    function onGoogleChartsLoad() {
        $(function() {
            params = getParams();

            charts.draw.all();
        });
    }
})();