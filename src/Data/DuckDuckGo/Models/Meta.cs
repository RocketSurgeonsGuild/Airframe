using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo;

/// <summary>
/// Represents meta data.
/// </summary>
public class Meta
{
    /// <summary>
    /// Gets or sets the attribution.
    /// </summary>
    public object Attribution { get; set; }

    /// <summary>
    /// Gets or sets the block group.
    /// </summary>
    public object Blockgroup { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public object CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the designer.
    /// </summary>
    public object Designer { get; set; }

    /// <summary>
    /// Gets or sets the dev date.
    /// </summary>
    public object DevDate { get; set; }

    /// <summary>
    /// Gets or sets the dev milestone.
    /// </summary>
    public string DevMilestone { get; set; }

    /// <summary>
    /// Gets or sets the attribution developer.
    /// </summary>
    public IEnumerable<Developer> Developer { get; set; }

    /// <summary>
    /// Gets or sets the example query.
    /// </summary>
    public string ExampleQuery { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the is stack exchange.
    /// </summary>
    public object IsStackexchange { get; set; }

    /// <summary>
    /// Gets or sets the java script call back name.
    /// </summary>
    public string JsCallbackName { get; set; }

    /// <summary>
    /// Gets or sets the live date.
    /// </summary>
    public object LiveDate { get; set; }

    /// <summary>
    /// Gets or sets the maintainer.
    /// </summary>
    public Maintainer Maintainer { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the perl module.
    /// </summary>
    public string PerlModule { get; set; }

    /// <summary>
    /// Gets or sets the producer.
    /// </summary>
    public object Producer { get; set; }

    /// <summary>
    /// Gets or sets the production state.
    /// </summary>
    public string ProductionState { get; set; }

    /// <summary>
    /// Gets or sets the repo.
    /// </summary>
    public string Repo { get; set; }

    /// <summary>
    /// Gets or sets the signal from.
    /// </summary>
    public string SignalFrom { get; set; }

    /// <summary>
    /// Gets or sets the source domain.
    /// </summary>
    public string SrcDomain { get; set; }

    /// <summary>
    /// Gets or sets the source id.
    /// </summary>
    public int SrcId { get; set; }

    /// <summary>
    /// Gets or sets the source name.
    /// </summary>
    public string SrcName { get; set; }

    /// <summary>
    /// Gets or sets the source options.
    /// </summary>
    public SrcOptions SrcOptions { get; set; }

    /// <summary>
    /// Gets or sets the source url.
    /// </summary>
    public string SrcUrl { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the tab.
    /// </summary>
    public string Tab { get; set; }

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    public IEnumerable<string> Topic { get; set; }

    /// <summary>
    /// Gets or sets the attribution.
    /// </summary>
    public int Unsafe { get; set; }
}