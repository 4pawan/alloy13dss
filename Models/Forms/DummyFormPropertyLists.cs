using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace alloy13dss.Models.Forms;

[PropertyDefinitionType]
public class PropertyDummyFormJourneyStepList : PropertyList<DummyFormJourneyStep>
{
}

[PropertyDefinitionType]
public class PropertyDummyFormConditionList : PropertyList<DummyFormCondition>
{
}

[PropertyDefinitionType]
public class PropertyDummyFormBranchList : PropertyList<DummyFormBranch>
{
}
